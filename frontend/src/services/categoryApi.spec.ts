import { describe, it, expect, vi, beforeEach } from "vitest";
import { categoryApi } from "./categoryApi";
import { apiRoutes } from "@/config/apiRoutes";
import type { CategoryRequest } from "@/types/api/categoryTypes";

vi.mock("@/services/apiClient", () => ({
  api: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    patch: vi.fn(),
    del: vi.fn(),
  },
}));

import { api } from "@/services/apiClient";

describe("categoryApi", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("getCategories", () => {
    it("calls api.get with list route and query", async () => {
      const query = { page: 1, pageSize: 10 };

      await categoryApi.getCategories(query);

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.list, {
        query,
      });
    });

    it("calls api.get with undefined query when not provided", async () => {
      await categoryApi.getCategories();

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.list, {
        query: undefined,
      });
    });
  });

  describe("getCategoryById", () => {
    it("calls api.get with byId route", async () => {
      await categoryApi.getCategoryById(5);

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.byId(5));
    });
  });

  describe("postCategory", () => {
    it("calls api.post with create route and payload", async () => {
      const payload: CategoryRequest = {
        name: "Electronics",
        description: "Electronic devices",
        parentCategoryIds: [1, 2],
      };

      await categoryApi.postCategory(payload);

      expect(api.post).toHaveBeenCalledWith(
        apiRoutes.categories.create,
        payload,
      );
    });
  });

  describe("putCategory", () => {
    it("calls api.put with update route and payload", async () => {
      const payload: CategoryRequest = {
        name: "Updated Electronics",
        description: "Updated description",
        parentCategoryIds: [],
      };

      await categoryApi.putCategory(10, payload);

      expect(api.put).toHaveBeenCalledWith(
        apiRoutes.categories.update(10),
        payload,
      );
    });
  });

  describe("toggleCategory", () => {
    it("calls api.patch with toggle route", async () => {
      await categoryApi.toggleCategory(7);

      expect(api.patch).toHaveBeenCalledWith(apiRoutes.categories.toggle(7));
    });
  });

  describe("getRoots", () => {
    it("calls api.get with roots route and default includeInactive=false", async () => {
      await categoryApi.getRoots();

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.roots, {
        query: { includeInactive: false },
      });
    });

    it("calls api.get with roots route and includeInactive=true", async () => {
      await categoryApi.getRoots(true);

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.roots, {
        query: { includeInactive: true },
      });
    });
  });

  describe("getChildren", () => {
    it("calls api.get with children route and default includeInactive=false", async () => {
      await categoryApi.getChildren(5);

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.children(5), {
        query: { includeInactive: false },
      });
    });

    it("calls api.get with children route and includeInactive=true", async () => {
      await categoryApi.getChildren(5, true);

      expect(api.get).toHaveBeenCalledWith(apiRoutes.categories.children(5), {
        query: { includeInactive: true },
      });
    });
  });

  describe("attachChild", () => {
    it("calls api.post with attachChild route", async () => {
      await categoryApi.attachChild(1, 99);

      expect(api.post).toHaveBeenCalledWith(
        apiRoutes.categories.attachChild(1, 99),
      );
    });
  });

  describe("detachChild", () => {
    it("calls api.del with detachChild route", async () => {
      await categoryApi.detachChild(1, 99);

      expect(api.del).toHaveBeenCalledWith(
        apiRoutes.categories.detachChild(1, 99),
      );
    });
  });
});
