import { describe, it, expect, vi, beforeEach } from "vitest";
import { productsApi } from "./productsApi";
import { apiRoutes } from "@/config/apiRoutes";
import type { ProductRequest } from "@/types/api/productTypes";

vi.mock("@/services/apiClient", () => ({
  api: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    patch: vi.fn(),
  },
}));

import { api } from "@/services/apiClient";
import { DEFAULT_QUERY } from "@/types/api/sharedApiTypes";

describe("productsApi", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("getProducts calls api.get with list route + query", async () => {
    await productsApi.getProducts({
      searchTerm: "lap",
      page: 1,
      ...DEFAULT_QUERY,
    });

    expect(api.get).toHaveBeenCalledWith(apiRoutes.products.list, {
      query: { searchTerm: "lap", page: 1, ...DEFAULT_QUERY },
    });
  });

  it("getProductById calls api.get with byId route", async () => {
    await productsApi.getProductById(10);

    expect(api.get).toHaveBeenCalledWith(apiRoutes.products.byId(10));
  });

  it("postProduct calls api.post with create route + body", async () => {
    const req: ProductRequest = {
      categoryId: 1,
      name: "A",
      description: "B",
      price: 10,
      stockQuantity: 2,
    };

    await productsApi.postProduct(req);

    expect(api.post).toHaveBeenCalledWith(apiRoutes.products.create, req);
  });

  it("putProduct calls api.put with update route + body", async () => {
    const req: ProductRequest = {
      categoryId: 1,
      name: "A",
      description: "B",
      price: 10,
      stockQuantity: 2,
    };

    await productsApi.putProduct(req, 5);

    expect(api.put).toHaveBeenCalledWith(apiRoutes.products.update(5), req);
  });

  it("patchToggleProductStatus calls api.patch with toggle route", async () => {
    await productsApi.patchToggleProductStatus(7);

    expect(api.patch).toHaveBeenCalledWith(apiRoutes.products.toggle(7));
  });
});
