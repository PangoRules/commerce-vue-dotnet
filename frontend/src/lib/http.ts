export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export type ApiError = {
  kind: "http" | "network" | "timeout" | "parse";
  message: string;
  status?: number;
  url?: string;
  method?: HttpMethod;
  requestId?: string;
  data?: unknown;
};

export type ApiOk<T> = {
  ok: true;
  status: number;
  headers: Headers;
  data: T;
};

export type ApiFail = {
  ok: false;
  status?: number;
  headers?: Headers;
  error: ApiError;
};

export type ApiResult<T> = ApiOk<T> | ApiFail;

export type RequestOptions = {
  method?: HttpMethod;
  headers?: Record<string, string>;
  query?: Record<string, string | number | boolean | null | undefined>;
  body?: unknown; // will JSON.stringify unless it's FormData/Blob/string
  timeoutMs?: number;
  signal?: AbortSignal;
  // if you want to override baseURL per call
  baseUrl?: string;
};

function buildUrl(
  baseUrl: string,
  path: string,
  query?: RequestOptions["query"],
) {
  const url = new URL(path, baseUrl);

  if (query) {
    for (const [k, v] of Object.entries(query)) {
      if (v === null || v === undefined) continue;
      url.searchParams.set(k, String(v));
    }
  }

  return url.toString();
}

function isFormData(v: unknown): v is FormData {
  return typeof FormData !== "undefined" && v instanceof FormData;
}
function isBlob(v: unknown): v is Blob {
  return typeof Blob !== "undefined" && v instanceof Blob;
}

async function safeReadText(res: Response) {
  try {
    return await res.text();
  } catch {
    return "";
  }
}

async function safeReadJson(res: Response) {
  try {
    return await res.json();
  } catch {
    return undefined;
  }
}

function mergeAbortSignals(a?: AbortSignal, b?: AbortSignal) {
  if (!a) return b;
  if (!b) return a;
  const ctrl = new AbortController();
  const onAbort = () => ctrl.abort();
  a.addEventListener("abort", onAbort);
  b.addEventListener("abort", onAbort);
  return ctrl.signal;
}

export function createHttpClient(config?: {
  baseUrl?: string;
  defaultHeaders?: Record<string, string>;
  // allow attaching auth tokens later
  getAuthHeader?: () => string | null;
}) {
  const baseUrl =
    config?.baseUrl ??
    (import.meta.env.VITE_API_BASE_URL as string | undefined) ??
    "http://localhost:8080";

  const defaultHeaders = config?.defaultHeaders ?? {};

  async function request<TResponse = unknown>(
    path: string,
    opts: RequestOptions = {},
  ): Promise<ApiResult<TResponse>> {
    const method = (opts.method ?? "GET") as HttpMethod;
    const url = buildUrl(opts.baseUrl ?? baseUrl, path, opts.query);

    const ctrl = new AbortController();
    const timeoutMs = opts.timeoutMs ?? 15000;

    const timeout = window.setTimeout(() => ctrl.abort(), timeoutMs);
    const signal = mergeAbortSignals(opts.signal, ctrl.signal);

    const headers: Record<string, string> = {
      ...defaultHeaders,
      ...opts.headers,
    };

    const auth = config?.getAuthHeader?.();
    if (auth) headers["Authorization"] = auth;

    // Body handling
    let body: BodyInit | undefined = undefined;
    if (opts.body !== undefined && opts.body !== null) {
      if (typeof opts.body === "string") {
        body = opts.body;
      } else if (isFormData(opts.body) || isBlob(opts.body)) {
        body = opts.body;
        // Don't set content-type; browser will for FormData
      } else {
        body = JSON.stringify(opts.body);
        headers["Content-Type"] ??= "application/json";
      }
    }

    try {
      const res = await fetch(url, {
        method,
        headers,
        body,
        signal,
      });

      window.clearTimeout(timeout);

      // Grab a correlation/request id if your backend emits one
      const requestId =
        res.headers.get("x-request-id") ??
        res.headers.get("x-correlation-id") ??
        undefined;

      // 204 No Content
      if (res.status === 204) {
        return {
          ok: true,
          status: res.status,
          headers: res.headers,
          data: undefined as TResponse,
        };
      }

      const contentType = res.headers.get("content-type") ?? "";
      const isJson = contentType.includes("application/json");

      if (res.ok) {
        if (isJson) {
          const json = (await safeReadJson(res)) as TResponse;
          return {
            ok: true,
            status: res.status,
            headers: res.headers,
            data: json,
          };
        }

        // non-json success (text)
        const text = (await safeReadText(res)) as unknown as TResponse;
        return {
          ok: true,
          status: res.status,
          headers: res.headers,
          data: text,
        };
      }

      // Error responses
      const errPayload = isJson
        ? await safeReadJson(res)
        : await safeReadText(res);

      return {
        ok: false,
        status: res.status,
        headers: res.headers,
        error: {
          kind: "http",
          message: `Request failed (${res.status})`,
          status: res.status,
          url,
          method,
          requestId,
          data: errPayload,
        },
      };
    } catch (e) {
      window.clearTimeout(timeout);

      const isAbort = e instanceof DOMException && e.name === "AbortError";
      return {
        ok: false,
        error: {
          kind: isAbort ? "timeout" : "network",
          message: isAbort
            ? `Request timed out after ${timeoutMs}ms`
            : "Network error",
          url,
          method,
        },
      };
    }
  }

  return {
    request,
    get<T = unknown>(
      path: string,
      opts?: Omit<RequestOptions, "method" | "body">,
    ) {
      return request<T>(path, { ...opts, method: "GET" });
    },
    post<T = unknown, B = unknown>(
      path: string,
      body?: B,
      opts?: Omit<RequestOptions, "method" | "body">,
    ) {
      return request<T>(path, { ...opts, method: "POST", body });
    },
    put<T = unknown, B = unknown>(
      path: string,
      body?: B,
      opts?: Omit<RequestOptions, "method" | "body">,
    ) {
      return request<T>(path, { ...opts, method: "PUT", body });
    },
    del<T = unknown>(
      path: string,
      opts?: Omit<RequestOptions, "method" | "body">,
    ) {
      return request<T>(path, { ...opts, method: "DELETE" });
    },
    baseUrl,
  };
}

// Optional helper if you prefer “throw on error” in some places
export function unwrap<T>(r: ApiResult<T>): T {
  if (r.ok) return r.data;
  throw Object.assign(new Error(r.error.message), { apiError: r.error });
}
