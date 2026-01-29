import { describe, it, expect, vi, beforeEach } from "vitest";
import { useProducts } from "./useProducts";
import type { ApiResult } from "@/lib/http";
import type { ProductMap, ProductResponse } from "@/types/api/productTypes";

vi.mock("@/services/productsApi", () => ({
  productsApi: {
    getProducts: vi.fn(),
    getProductById: vi.fn(),
    postProduct: vi.fn(),
    putProduct: vi.fn(),
    patchToggleProductStatus: vi.fn(),
  },
}));

import { productsApi } from "@/services/productsApi";

function ok<T>(data: T): ApiResult<T> {
  return { ok: true, status: 200, headers: new Headers(), data };
}

describe("useProducts", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("loadProductList calls getProducts and stores result", async () => {
    const data: ProductMap = {};
    (
      productsApi.getProducts as unknown as ReturnType<typeof vi.fn>
    ).mockResolvedValue(ok(data));

    const sut = useProducts();

    await sut.loadProductList({ searchTerm: "laptop" });

    expect(productsApi.getProducts).toHaveBeenCalledWith({
      searchTerm: "laptop",
    });
    expect(sut.listProductResult.value?.ok).toBe(true);
    expect(sut.isProductListLoading.value).toBe(false);
  });

  it("loadByProductId calls getProductById and stores result", async () => {
    const product: ProductResponse = {
      id: 1,
      categoryId: 2,
      name: "Laptop",
      description: null,
      price: 1000,
      stockQuantity: 5,
      isActive: true,
      category: null,
      images: [],
      primaryImageUrl: null,
    };
    (
      productsApi.getProductById as unknown as ReturnType<typeof vi.fn>
    ).mockResolvedValue(ok(product));

    const sut = useProducts();

    await sut.loadByProductId(1);

    expect(productsApi.getProductById).toHaveBeenCalledWith(1);
    expect(sut.byProductIdResult.value?.ok).toBe(true);
    expect(sut.isByProductIdLoading.value).toBe(false);
  });

  it("createProduct calls postProduct and stores result", async () => {
    (
      productsApi.postProduct as unknown as ReturnType<typeof vi.fn>
    ).mockResolvedValue(ok(undefined));

    const sut = useProducts();

    const req = {
      categoryId: 1,
      name: "Test",
      description: "Desc",
      price: 10,
      stockQuantity: 2,
    };

    await sut.createProduct(req);

    expect(productsApi.postProduct).toHaveBeenCalledWith(req);
    expect(sut.createdProductResult.value?.ok).toBe(true);
    expect(sut.isProductCreatedLoading.value).toBe(false);
  });

  it("updateProduct calls putProduct and stores result", async () => {
    const updated: ProductResponse = {
      id: 1,
      categoryId: 1,
      name: "Updated",
      description: "Desc",
      price: 11,
      stockQuantity: 3,
      isActive: true,
      category: null,
      images: [],
      primaryImageUrl: null,
    };
    (
      productsApi.putProduct as unknown as ReturnType<typeof vi.fn>
    ).mockResolvedValue(ok(updated));

    const sut = useProducts();

    const req = {
      categoryId: 1,
      name: "Updated",
      description: "Desc",
      price: 11,
      stockQuantity: 3,
    };

    await sut.updateProduct(req, 1);

    expect(productsApi.putProduct).toHaveBeenCalledWith(req, 1);
    expect(sut.updatedProductResult.value?.ok).toBe(true);
    expect(sut.isUpdatedProductLoading.value).toBe(false);
  });

  it("toggleProductStatus calls patchToggleProductStatus and stores result", async () => {
    (
      productsApi.patchToggleProductStatus as unknown as ReturnType<
        typeof vi.fn
      >
    ).mockResolvedValue(ok(undefined));

    const sut = useProducts();

    await sut.toggleProductStatus(7);

    expect(productsApi.patchToggleProductStatus).toHaveBeenCalledWith(7);
    expect(sut.patchToggleProductStatusResult.value?.ok).toBe(true);
    expect(sut.isPatchToggleProductStatusLoading.value).toBe(false);
  });

  it("sets loading true while list request is in flight", async () => {
    let resolve!: (v: ApiResult<ProductMap>) => void;

    (
      productsApi.getProducts as unknown as ReturnType<typeof vi.fn>
    ).mockImplementation(() => new Promise((r) => (resolve = r)));

    const sut = useProducts();

    const p = sut.loadProductList();
    expect(sut.isProductListLoading.value).toBe(true);

    resolve(ok({}));

    await p;
    expect(sut.isProductListLoading.value).toBe(false);
  });
});
