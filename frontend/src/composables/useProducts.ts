import { ref } from "vue";
import type { ApiResult } from "@/lib/http";
import type {
  ProductResponse,
  ProductListQuery,
  ProductMap,
  ProductRequest,
} from "@/types/api/products";
import { productsApi } from "@/services/productsApi";

export function useProducts() {
  const listProductResult = ref<ApiResult<ProductMap> | null>(null);
  const isProductListLoading = ref(false);
  async function loadProductList(query?: ProductListQuery) {
    isProductListLoading.value = true;
    listProductResult.value = await productsApi.getProducts(query);
    isProductListLoading.value = false;
  }

  const byProductIdResult = ref<ApiResult<ProductResponse> | null>(null);
  const isByProductIdLoading = ref(false);
  async function loadByProductId(id: number) {
    isByProductIdLoading.value = true;
    byProductIdResult.value = await productsApi.getProductById(id);
    isByProductIdLoading.value = false;
  }

  const createdProductResult = ref<ApiResult<null | void> | null>(null);
  const isProductCreatedLoading = ref(false);
  async function createProduct(request: ProductRequest) {
    isProductCreatedLoading.value = true;
    createdProductResult.value = await productsApi.postProduct(request);
    isProductCreatedLoading.value = false;
  }

  const updatedProductResult = ref<ApiResult<ProductResponse> | null>(null);
  const isUpdatedProductLoading = ref(false);
  async function updateProduct(request: ProductRequest, id: number) {
    isUpdatedProductLoading.value = true;
    updatedProductResult.value = await productsApi.putProduct(request, id);
    isUpdatedProductLoading.value = false;
  }

  const patchToggleProductStatusResult = ref<ApiResult<null | void> | null>(
    null,
  );
  const isPatchToggleProductStatusLoading = ref(false);
  async function toggleProductStatus(id: number) {
    isPatchToggleProductStatusLoading.value = true;
    patchToggleProductStatusResult.value =
      await productsApi.patchToggleProductStatus(id);
    isPatchToggleProductStatusLoading.value = false;
  }

  return {
    listProductResult,
    isProductListLoading,
    loadProductList,

    byProductIdResult,
    isByProductIdLoading,
    loadByProductId,

    createdProductResult,
    isProductCreatedLoading,
    createProduct,

    updatedProductResult,
    isUpdatedProductLoading,
    updateProduct,

    patchToggleProductStatusResult,
    isPatchToggleProductStatusLoading,
    toggleProductStatus,
  };
}
