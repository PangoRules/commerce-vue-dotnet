import { describe, it, expect, vi, beforeEach } from "vitest";
import { createHttpClient, unwrap } from "./http";

function jsonResponse(
  body: unknown,
  init?: ResponseInit & { headers?: Record<string, string> },
) {
  const headers = new Headers({
    "content-type": "application/json",
    ...(init?.headers ?? {}),
  });
  return new Response(JSON.stringify(body), { ...init, headers });
}

function textResponse(
  body: string,
  init?: ResponseInit & { headers?: Record<string, string> },
) {
  const headers = new Headers({
    "content-type": "text/plain",
    ...(init?.headers ?? {}),
  });
  return new Response(body, { ...init, headers });
}

describe("http client", () => {
  const baseUrl = "http://example.test";

  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it("builds url with query params (including arrays) and calls fetch", async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValue(jsonResponse({ ok: true }, { status: 200 }));
    vi.stubGlobal("fetch", fetchMock);

    const api = createHttpClient({ baseUrl });

    await api.get("/api/products", {
      query: { search: "laptop", tags: ["a", "b"], page: 2, nope: undefined },
    });

    expect(fetchMock).toHaveBeenCalledTimes(1);

    const url = fetchMock.mock.calls[0]![0] as string;
    expect(String(url)).toContain("http://example.test/api/products?");
    expect(String(url)).toContain("search=laptop");
    expect(String(url)).toContain("page=2");
    expect(String(url)).toContain("tags=a");
    expect(String(url)).toContain("tags=b");
    expect(String(url)).not.toContain("nope=");
  });

  it("parses json success response", async () => {
    vi.stubGlobal(
      "fetch",
      vi
        .fn()
        .mockResolvedValue(jsonResponse({ status: "ok" }, { status: 200 })),
    );

    const api = createHttpClient({ baseUrl });
    const res = await api.get<{ status: string }>("/api/health");

    expect(res.ok).toBe(true);
    if (res.ok) expect(res.data.status).toBe("ok");
  });

  it("parses text success response when not json", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(textResponse("hello", { status: 200 })),
    );

    const api = createHttpClient({ baseUrl });
    const res = await api.get<string>("/api/ping");

    expect(res.ok).toBe(true);
    if (res.ok) expect(res.data).toBe("hello");
  });

  it("handles 204 No Content", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(new Response(null, { status: 204 })),
    );

    const api = createHttpClient({ baseUrl });
    const res = await api.post<void>("/api/products", {});

    expect(res.ok).toBe(true);
    if (res.ok) expect(res.data).toBeUndefined();
  });

  it("returns ApiFail for http error and extracts request id + message", async () => {
    vi.stubGlobal(
      "fetch",
      vi
        .fn()
        .mockResolvedValue(
          jsonResponse(
            { title: "Bad Request" },
            { status: 400, headers: { "x-request-id": "abc-123" } },
          ),
        ),
    );

    const onError = vi.fn();
    const api = createHttpClient({ baseUrl, onError });

    const res = await api.get("/api/products");

    expect(res.ok).toBe(false);
    if (!res.ok) {
      expect(res.error.kind).toBe("http");
      expect(res.error.status).toBe(400);
      expect(res.error.requestId).toBe("abc-123");
      expect(res.error.message).toBe("Bad Request");
    }
    expect(onError).toHaveBeenCalledTimes(1);
  });

  it("does not call onError when opts.silent is true", async () => {
    vi.stubGlobal(
      "fetch",
      vi
        .fn()
        .mockResolvedValue(jsonResponse({ title: "Nope" }, { status: 500 })),
    );

    const onError = vi.fn();
    const api = createHttpClient({ baseUrl, onError });

    await api.get("/api/products", { silent: true });

    expect(onError).not.toHaveBeenCalled();
  });

  it("string body is sent as-is (no content-type forced)", async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValue(textResponse("ok", { status: 200 }));
    vi.stubGlobal("fetch", fetchMock);

    const api = createHttpClient({ baseUrl });
    await api.post("/api/raw", "plain text");

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const init = fetchMock.mock.calls[0]![1] as RequestInit;
    expect(init.method).toBe("POST");
    expect(init.body).toBe("plain text");
  });

  it("object body is JSON.stringified and sets content-type", async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValue(jsonResponse({ ok: true }, { status: 200 }));
    vi.stubGlobal("fetch", fetchMock);

    const api = createHttpClient({ baseUrl });
    await api.post("/api/products", { name: "x" });

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const init = fetchMock.mock.calls[0]![1] as RequestInit;
    expect(init.method).toBe("POST");
    expect(init.body).toBe(JSON.stringify({ name: "x" }));
    expect((init.headers as Record<string, string>)["Content-Type"]).toBe(
      "application/json",
    );
  });

  it("timeout returns kind=timeout", async () => {
    // make setTimeout fire immediately and abort request
    vi.useFakeTimers();

    vi.stubGlobal(
      "fetch",
      vi.fn().mockImplementation((_url: string, init: RequestInit) => {
        return new Promise((_resolve, reject) => {
          init.signal?.addEventListener("abort", () => {
            reject(new DOMException("Aborted", "AbortError"));
          });
        });
      }),
    );

    const api = createHttpClient({ baseUrl });
    const p = api.get("/api/slow", { timeoutMs: 1 });

    vi.runAllTimers();

    const res = await p;

    expect(res.ok).toBe(false);
    if (!res.ok) {
      expect(res.error.kind).toBe("timeout");
    }

    vi.useRealTimers();
  });

  it("unwrap returns data for ok and throws for fail", async () => {
    expect(
      unwrap({ ok: true, status: 200, headers: new Headers(), data: 123 }),
    ).toBe(123);

    expect(() =>
      unwrap({ ok: false, error: { kind: "http", message: "nope" } }),
    ).toThrow("nope");
  });
});
