import { describe, it, expect, vi, beforeEach } from "vitest";
import { useProductImages } from "./useProductImages";
import type { ApiResult } from "@/lib/http";
import type { ProductImageResponse } from "@/types/api/productTypes";

vi.mock("@/services/productImageApi", () => ({
  productImageApi: {
    getImages: vi.fn(),
    getImageMetadata: vi.fn(),
    getImageUrl: vi.fn(),
    uploadImage: vi.fn(),
    deleteImage: vi.fn(),
    setPrimary: vi.fn(),
    reorderImages: vi.fn(),
  },
}));

import { productImageApi } from "@/services/productImageApi";

function ok<T>(data: T): ApiResult<T> {
  return { ok: true, status: 200, headers: new Headers(), data };
}

function okVoid(): ApiResult<void> {
  return { ok: true, status: 204, headers: new Headers(), data: undefined };
}

const mockImage: ProductImageResponse = {
  id: "img-123",
  productId: 1,
  fileName: "test.jpg",
  contentType: "image/jpeg",
  sizeBytes: 1024,
  displayOrder: 0,
  isPrimary: true,
  uploadedAt: "2026-01-01T00:00:00Z",
  url: "/api/productimage/img-123",
};

describe("useProductImages", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("loadImages", () => {
    it("calls getImages and stores result", async () => {
      const images = [mockImage];
      (
        productImageApi.getImages as unknown as ReturnType<typeof vi.fn>
      ).mockResolvedValue(ok(images));

      const sut = useProductImages();
      await sut.loadImages(1);

      expect(productImageApi.getImages).toHaveBeenCalledWith(1);
      expect(sut.imagesResult.value?.ok).toBe(true);
      expect(sut.isImagesLoading.value).toBe(false);
    });

    it("sets loading true while request is in flight", async () => {
      let resolve!: (v: ApiResult<ProductImageResponse[]>) => void;
      (
        productImageApi.getImages as unknown as ReturnType<typeof vi.fn>
      ).mockImplementation(() => new Promise((r) => (resolve = r)));

      const sut = useProductImages();
      const p = sut.loadImages(1);

      expect(sut.isImagesLoading.value).toBe(true);

      resolve(ok([]));
      await p;

      expect(sut.isImagesLoading.value).toBe(false);
    });
  });

  describe("loadImageMetadata", () => {
    it("calls getImageMetadata and stores result", async () => {
      (
        productImageApi.getImageMetadata as unknown as ReturnType<typeof vi.fn>
      ).mockResolvedValue(ok(mockImage));

      const sut = useProductImages();
      await sut.loadImageMetadata("img-123");

      expect(productImageApi.getImageMetadata).toHaveBeenCalledWith("img-123");
      expect(sut.imageMetadataResult.value?.ok).toBe(true);
      expect(sut.isImageMetadataLoading.value).toBe(false);
    });
  });

  describe("uploadImage", () => {
    it("calls uploadImage and stores result", async () => {
      (
        productImageApi.uploadImage as unknown as ReturnType<typeof vi.fn>
      ).mockResolvedValue(ok(mockImage));

      const sut = useProductImages();
      const file = new File(["content"], "photo.jpg", { type: "image/jpeg" });

      const result = await sut.uploadImage(1, file);

      expect(productImageApi.uploadImage).toHaveBeenCalledWith(1, file);
      expect(result?.ok).toBe(true);
      expect(sut.isUploading.value).toBe(false);
    });

    it("sets isUploading true while uploading", async () => {
      let resolve!: (v: ApiResult<ProductImageResponse>) => void;
      (
        productImageApi.uploadImage as unknown as ReturnType<typeof vi.fn>
      ).mockImplementation(() => new Promise((r) => (resolve = r)));

      const sut = useProductImages();
      const file = new File(["content"], "photo.jpg", { type: "image/jpeg" });
      const p = sut.uploadImage(1, file);

      expect(sut.isUploading.value).toBe(true);

      resolve(ok(mockImage));
      await p;

      expect(sut.isUploading.value).toBe(false);
    });
  });

  describe("deleteImage", () => {
    it("calls deleteImage and stores result", async () => {
      (
        productImageApi.deleteImage as unknown as ReturnType<typeof vi.fn>
      ).mockResolvedValue(okVoid());

      const sut = useProductImages();
      const result = await sut.deleteImage("img-123");

      expect(productImageApi.deleteImage).toHaveBeenCalledWith("img-123");
      expect(result?.ok).toBe(true);
      expect(sut.isDeleting.value).toBe(false);
    });
  });

  describe("setPrimary", () => {
    it("calls setPrimary and stores result", async () => {
      (
        productImageApi.setPrimary as unknown as ReturnType<typeof vi.fn>
      ).mockResolvedValue(okVoid());

      const sut = useProductImages();
      const result = await sut.setPrimary("img-456");

      expect(productImageApi.setPrimary).toHaveBeenCalledWith("img-456");
      expect(result?.ok).toBe(true);
      expect(sut.isSettingPrimary.value).toBe(false);
    });
  });

  describe("reorderImages", () => {
    it("calls reorderImages and stores result", async () => {
      (
        productImageApi.reorderImages as unknown as ReturnType<typeof vi.fn>
      ).mockResolvedValue(okVoid());

      const sut = useProductImages();
      const ids = ["id-1", "id-2", "id-3"];
      const result = await sut.reorderImages(1, ids);

      expect(productImageApi.reorderImages).toHaveBeenCalledWith(1, ids);
      expect(result?.ok).toBe(true);
      expect(sut.isReordering.value).toBe(false);
    });
  });

  describe("getImageUrl", () => {
    it("returns URL from productImageApi", () => {
      (
        productImageApi.getImageUrl as unknown as ReturnType<typeof vi.fn>
      ).mockReturnValue("/api/productimage/xyz");

      const sut = useProductImages();
      const url = sut.getImageUrl("xyz");

      expect(url).toBe("/api/productimage/xyz");
      expect(productImageApi.getImageUrl).toHaveBeenCalledWith("xyz");
    });
  });
});
