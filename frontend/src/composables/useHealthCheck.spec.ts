import { describe, it, expect, vi } from "vitest";
import { useHealthCheck } from "./useHealthCheck";

vi.mock("@/services/apiClient", () => ({
  api: {
    get: vi.fn(),
  },
}));

import { api } from "@/services/apiClient";

describe("useHealthCheck", () => {
  it("calls /api/health and stores result", async () => {
    (api.get as unknown as ReturnType<typeof vi.fn>).mockResolvedValue({
      ok: true,
      status: 200,
      headers: new Headers(),
      data: { status: "ok" },
    });

    const sut = useHealthCheck();

    await sut.check();

    expect(api.get).toHaveBeenCalledWith("/api/health");
    expect(sut.result.value?.ok).toBe(true);
    expect(sut.isLoading.value).toBe(false);
  });

  it("sets isLoading true while request is in flight", async () => {
    let resolve!: (v: unknown) => void;

    (api.get as unknown as ReturnType<typeof vi.fn>).mockImplementation(
      () => new Promise((r) => (resolve = r)),
    );

    const sut = useHealthCheck();

    const p = sut.check();
    expect(sut.isLoading.value).toBe(true);

    resolve({
      ok: true,
      status: 200,
      headers: new Headers(),
      data: { status: "ok" },
    });

    await p;

    expect(sut.isLoading.value).toBe(false);
  });
});
