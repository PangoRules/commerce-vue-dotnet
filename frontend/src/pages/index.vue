<template>
  <div>
    <h1>Vue 3 + Vite + Vuetify ✅</h1>

    <v-alert type="success" variant="tonal" class="mt-4">
      Frontend scaffold is running.
    </v-alert>

    <v-alert type="info" class="mt-2">
      {{ statusText }}
    </v-alert>

    <!-- Products -->
    <div class="mt-6">
      <h2>Products</h2>
      <v-btn color="primary" class="mt-4" @click="createProductBtn">
        Create Test Product
      </v-btn>

      <!-- Loading -->
      <v-progress-linear
        v-if="isProductListLoading"
        indeterminate
        class="mb-2"
      />

      <!-- Error -->
      <v-alert
        v-else-if="listProductResult && !listProductResult.ok"
        type="error"
        variant="tonal"
      >
        {{ listProductResult.error.message }}
      </v-alert>

      <!-- Success -->
      <v-list v-else-if="listProductResult?.ok">
        <v-list-item v-for="product in products" :key="product.id">
          <v-list-item-title>
            {{ product.name }}
          </v-list-item-title>

          <v-list-item-subtitle>
            ${{ product.price }} · Stock: {{ product.stockQuantity }}
          </v-list-item-subtitle>
        </v-list-item>
      </v-list>

      <!-- Empty -->
      <v-alert v-else type="warning" variant="tonal">
        No products found
      </v-alert>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useHealthCheck } from "@/composables/useHealthCheck";
import { computed, onMounted } from "vue";
import { useProducts } from "@/composables/useProducts";
import type {
  ProductListQuery,
  ProductRequest,
  ProductResponse,
} from "@/types/api/productTypes";

const { result, check } = useHealthCheck();

const statusText = computed(() => {
  if (!result.value) return "not checked";
  return result.value.ok
    ? result.value.data.status
    : result.value.error.message;
});

const {
  loadProductList,
  listProductResult,
  isProductListLoading,
  createProduct,
  createdProductResult,
} = useProducts();

const products = computed<ProductResponse[]>(() => {
  if (!listProductResult.value?.ok) return [];
  return Object.values(listProductResult.value.data);
});

const createProductBtn = async () => {
  const request: ProductRequest = {
    name: "Test Product",
    description: "Created from Vue UI",
    price: 19.99,
    stockQuantity: 10,
    categoryId: 1,
  };

  await createProduct(request);

  if (createdProductResult.value?.ok) {
    // reload products after create
    await localLoadList();
  }
};

const localLoadList = async () => {
  const getProductsQuery: ProductListQuery = {
    pageSize: 50,
  };

  await loadProductList(getProductsQuery);
};

onMounted(async () => {
  check();
  await localLoadList();
});
</script>
