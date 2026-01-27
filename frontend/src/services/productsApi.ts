import { api } from "@/services/apiClient";
import { apiRoutes } from "@/config/apiRoutes";
import type {
  ProductResponse,
  ProductListQuery,
  ProductMap,
  ProductRequest,
} from "@/types/api/products";

export const productsApi = {
  getProducts(query?: ProductListQuery) {
    return api.get<ProductMap>(apiRoutes.products.list, { query });
  },
  getProductById(id: number) {
    return api.get<ProductResponse>(apiRoutes.products.byId(id));
  },
  postProduct(request: ProductRequest) {
    return api.post<void>(apiRoutes.products.create, request);
  },
  putProduct(request: ProductRequest, id: number) {
    return api.put<ProductResponse>(apiRoutes.products.update(id), request);
  },
  patchToggleProductStatus(id: number) {
    return api.patch<void>(apiRoutes.products.toggle(id));
  },
};
