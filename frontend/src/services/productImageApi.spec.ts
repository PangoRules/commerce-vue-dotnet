import { describe, it, expect, vi, beforeEach } from "vitest";
import { productImageApi } from "./productImageApi";
import { apiRoutes } from "@/config/apiRoutes";

vi.mock("@/services/apiClient", () => ({
  api: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    del: vi.fn(),
  },
}));

import { api } from "@/services/apiClient";

describe("productImageApi", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("getImages calls api.get with list route", async () => {
    await productImageApi.getImages(123);

    expect(api.get).toHaveBeenCalledWith(apiRoutes.productImages.list(123));
  });

  it("getImageMetadata calls api.get with metadata route", async () => {
    const imageId = "abc-123";

    await productImageApi.getImageMetadata(imageId);

    expect(api.get).toHaveBeenCalledWith(apiRoutes.productImages.metadata(imageId));
  });

  it("getImageUrl returns the image proxy URL", () => {
    const imageId = "def-456";

    const url = productImageApi.getImageUrl(imageId);

    expect(url).toBe(apiRoutes.productImages.get(imageId));
  });

  it("uploadImage calls api.post with FormData", async () => {
    const productId = 42;
    const file = new File(["test content"], "test.jpg", { type: "image/jpeg" });

    await productImageApi.uploadImage(productId, file);

    expect(api.post).toHaveBeenCalledWith(
      apiRoutes.productImages.upload(productId),
      expect.any(FormData),
      expect.objectContaining({
        headers: { "Content-Type": undefined },
      })
    );
  });

  it("deleteImage calls api.del with delete route", async () => {
    const imageId = "ghi-789";

    await productImageApi.deleteImage(imageId);

    expect(api.del).toHaveBeenCalledWith(apiRoutes.productImages.delete(imageId));
  });

  it("setPrimary calls api.put with setPrimary route", async () => {
    const imageId = "jkl-012";

    await productImageApi.setPrimary(imageId);

    expect(api.put).toHaveBeenCalledWith(apiRoutes.productImages.setPrimary(imageId));
  });

  it("reorderImages calls api.put with reorder route and request body", async () => {
    const productId = 99;
    const imageIds = ["id-1", "id-2", "id-3"];

    await productImageApi.reorderImages(productId, imageIds);

    expect(api.put).toHaveBeenCalledWith(
      apiRoutes.productImages.reorder(productId),
      { imageIds }
    );
  });
});
