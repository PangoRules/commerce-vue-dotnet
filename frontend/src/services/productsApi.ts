import { api } from "@/services/apiClient";
import type {
  ProductResponse,
  ProductListQuery,
  ProductMap,
} from "@/types/api/products";

export const productsApi = {
  getProducts(query?: ProductListQuery) {
    return api.get<ProductMap>("/api/product", { query });
  },
  getProductById(id: number) {
    return api.get<ProductResponse>(`/api/product/${id}`);
  },
};
