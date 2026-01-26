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

      <!-- Loading -->
      <v-progress-linear v-if="isListLoading" indeterminate class="mb-2" />

      <!-- Error -->
      <v-alert
        v-else-if="listResult && !listResult.ok"
        type="error"
        variant="tonal"
      >
        {{ listResult.error.message }}
      </v-alert>

      <!-- Success -->
      <v-list v-else-if="listResult?.ok">
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
import type { ProductResponse } from "@/types/api/products";

const { result, check } = useHealthCheck();

const statusText = computed(() => {
  if (!result.value) return "not checked";
  return result.value.ok
    ? result.value.data.status
    : result.value.error.message;
});

const { loadList, listResult, isListLoading } = useProducts();

const products = computed<ProductResponse[]>(() => {
  if (!listResult.value?.ok) return [];
  return Object.values(listResult.value.data);
});

onMounted(async () => {
  check();
  await loadList();
});
</script>
