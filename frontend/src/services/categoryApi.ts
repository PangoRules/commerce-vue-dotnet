import { api } from "@/services/apiClient";
import { apiRoutes } from "@/config/apiRoutes";
import type {
  CategoryAdminDetailsResponse,
  CategoryListQuery,
  CategoryRequest,
  CategoryResponse,
} from "@/types/api/categoryTypes";

export const categoryApi = {
  // GET /api/category?...
  getCategories(query?: CategoryListQuery) {
    return api.get<CategoryResponse[]>(apiRoutes.categories.list, { query });
  },

  // GET /api/category/{categoryId}
  getCategoryById(categoryId: number) {
    return api.get<CategoryAdminDetailsResponse>(
      apiRoutes.categories.byId(categoryId),
    );
  },

  // POST /api/category
  postCategory(payload: CategoryRequest) {
    return api.post<void>(apiRoutes.categories.create, payload);
  },

  // PUT /api/category/{categoryId}
  putCategory(categoryId: number, payload: CategoryRequest) {
    return api.put<void>(apiRoutes.categories.update(categoryId), payload);
  },

  // PATCH /api/category/toggle/{categoryId}
  toggleCategory(categoryId: number) {
    return api.patch<void>(apiRoutes.categories.toggle(categoryId));
  },

  // GET /api/category/roots?includeInactive=...
  getRoots(includeInactive = false) {
    return api.get<CategoryResponse[]>(apiRoutes.categories.roots, {
      query: { includeInactive },
    });
  },

  // GET /api/category/{parentCategoryId}/children?includeInactive=...
  getChildren(parentCategoryId: number, includeInactive = false) {
    return api.get<CategoryResponse[]>(
      apiRoutes.categories.children(parentCategoryId),
      {
        query: { includeInactive },
      },
    );
  },

  // POST /api/category/{parentCategoryId}/children/{childCategoryId}
  attachChild(parentCategoryId: number, childCategoryId: number) {
    return api.post<void>(
      apiRoutes.categories.attachChild(parentCategoryId, childCategoryId),
    );
  },

  // DELETE /api/category/{parentCategoryId}/children/{childCategoryId}
  detachChild(parentCategoryId: number, childCategoryId: number) {
    return api.del<void>(
      apiRoutes.categories.detachChild(parentCategoryId, childCategoryId),
    );
  },
};
