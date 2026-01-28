import type { ApiOk, ApiFail } from "@/lib/http";

export function httpOk<T>(data: T, status = 200): ApiOk<T> {
  return {
    ok: true,
    status,
    headers: new Headers(),
    data,
  };
}

export function httpFail(status = 500, message = "Request failed"): ApiFail {
  return {
    ok: false,
    status,
    headers: new Headers(),
    error: {
      kind: "http",
      message,
      status,
      url: "/fake",
      method: "GET",
    },
  };
}
