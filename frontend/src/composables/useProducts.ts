import { ref } from "vue";
import type { ApiResult } from "@/lib/http";
import type {
  ProductResponse,
  ProductListQuery,
  ProductMap,
} from "@/types/api/products";
import { productsApi } from "@/services/productsApi";

export function useProducts() {
  const listResult = ref<ApiResult<ProductMap> | null>(null);
  const isListLoading = ref(false);

  const byIdResult = ref<ApiResult<ProductResponse> | null>(null);
  const isByIdLoading = ref(false);

  async function loadList(query?: ProductListQuery) {
    isListLoading.value = true;
    listResult.value = await productsApi.getProducts(query);
    isListLoading.value = false;
  }

  async function loadById(id: number) {
    isByIdLoading.value = true;
    byIdResult.value = await productsApi.getProductById(id);
    isByIdLoading.value = false;
  }

  return {
    listResult,
    isListLoading,
    loadList,

    byIdResult,
    isByIdLoading,
    loadById,
  };
}
