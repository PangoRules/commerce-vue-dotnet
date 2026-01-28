import { describe, it, expect, vi, beforeEach } from "vitest";
import type { ApiError } from "@/lib/http";
import type { Pinia } from "pinia";
import { initNotify, notifyError, notifyApiError } from "./notify";

// mock toast store
const errorMock = vi.fn();

vi.mock("@/stores/toast", () => ({
  useToastStore: vi.fn(() => ({
    error: errorMock,
  })),
}));

import { useToastStore } from "@/stores/toast";

describe("notify", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  function fakePinia(): Pinia {
    return {} as Pinia;
  }

  it("does nothing if initNotify was not called", () => {
    expect(() => notifyError("boom")).not.toThrow();
    expect(errorMock).not.toHaveBeenCalled();
  });

  it("notifyError sends message to toast store", () => {
    initNotify(fakePinia());

    notifyError("Something broke");

    expect(useToastStore).toHaveBeenCalled();
    expect(errorMock).toHaveBeenCalledWith("Something broke");
  });

  it("notifyApiError maps timeout error", () => {
    initNotify(fakePinia());

    const err: ApiError = {
      kind: "timeout",
      message: "ignored",
    };

    notifyApiError(err);

    expect(errorMock).toHaveBeenCalledWith(
      "Request timed out. Please try again.",
    );
  });

  it("notifyApiError maps network error", () => {
    initNotify(fakePinia());

    const err: ApiError = {
      kind: "network",
      message: "ignored",
    };

    notifyApiError(err);

    expect(errorMock).toHaveBeenCalledWith(
      "Network error. Check your connection.",
    );
  });

  it("notifyApiError uses api message for http errors", () => {
    initNotify(fakePinia());

    const err: ApiError = {
      kind: "http",
      message: "Bad request",
      status: 400,
    };

    notifyApiError(err);

    expect(errorMock).toHaveBeenCalledWith("Bad request");
  });
});
