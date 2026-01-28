<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useCategories } from "@/composables/useCategories";
import type { CategoryResponse } from "@/types/api/categoryTypes";
import type { ProductResponse } from "@/types/api/productTypes";
import { ProductCategorySection } from "@/components/products";
import { LoadingSpinner, EmptyState } from "@/components/shared";

const { loadRoots, rootsResult, isRootsLoading } = useCategories();

const categories = computed<CategoryResponse[]>(() => {
  if (!rootsResult.value?.ok) return [];
  return rootsResult.value.data;
});

const hasCategories = computed(() => categories.value.length > 0);
const isLoading = computed(() => isRootsLoading.value);
const hasError = computed(() => rootsResult.value && !rootsResult.value.ok);

const handleAddToCart = (product: ProductResponse) => {
  // TODO: Implement cart functionality
  console.log("Add to cart:", product);
};

onMounted(() => {
  loadRoots();
});
</script>

<template>
  <v-container class="py-8">
    <!-- Hero Section -->
    <section class="hero-section text-center mb-12">
      <h1 class="text-h3 text-md-h2 font-weight-bold mb-4">
        Welcome to Commerce
      </h1>
      <p class="text-h6 text-medium-emphasis mx-auto" style="max-width: 600px">
        Discover amazing products across all categories.
        Quality items at great prices.
      </p>
    </section>

    <!-- Loading State -->
    <LoadingSpinner v-if="isLoading" text="Loading categories..." />

    <!-- Error State -->
    <v-alert v-else-if="hasError" type="error" variant="tonal" class="my-8">
      <template #title>Failed to load categories</template>
      <template #text>
        {{ rootsResult?.ok === false ? rootsResult.error.message : 'Unknown error' }}
      </template>
      <template #append>
        <v-btn variant="text" @click="loadRoots()">
          Retry
        </v-btn>
      </template>
    </v-alert>

    <!-- Empty State -->
    <EmptyState
      v-else-if="!hasCategories"
      title="No categories yet"
      description="Check back later for amazing products!"
      icon="mdi-store-outline"
    />

    <!-- Category Sections -->
    <template v-else>
      <ProductCategorySection
        v-for="category in categories"
        :key="category.id"
        :category="category"
        :limit="4"
        @add-to-cart="handleAddToCart"
      />
    </template>

    <!-- Footer CTA -->
    <section v-if="hasCategories" class="text-center mt-12 py-8">
      <h2 class="text-h5 mb-4">Can't find what you're looking for?</h2>
      <v-btn color="primary" size="large" to="/products">
        Browse All Products
        <v-icon end icon="mdi-arrow-right" />
      </v-btn>
    </section>
  </v-container>
</template>

<style scoped>
.hero-section {
  padding: 2rem 0;
}
</style>
