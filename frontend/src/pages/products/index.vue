<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useCategories } from "@/composables/useCategories";
import type { CategoryResponse } from "@/types/api/categoryTypes";
import type { ProductResponse } from "@/types/api/productTypes";
import { ProductCategorySection } from "@/components/products";
import { LoadingSpinner, EmptyState } from "@/components/shared";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
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
  <v-container class="py-8 mx-auto">
    <!-- Loading State -->
    <LoadingSpinner v-if="isLoading" text="Loading categories..." />

    <!-- Error State -->
    <v-alert v-else-if="hasError" type="error" variant="tonal" class="my-8">
      <template #title>{{ t("errors.categoriesLoadFailedTitle") }}</template>
      <template #text>
        {{
          rootsResult?.ok === false
            ? rootsResult.error.message
            : t("errors.unknown")
        }}
      </template>
      <template #append>
        <v-btn variant="text" @click="loadRoots()">
          {{ t("common.actions.retry") }}
        </v-btn>
      </template>
    </v-alert>

    <!-- Empty State -->
    <EmptyState
      v-else-if="!hasCategories"
      :title="t('products.empty.categoriesTitle')"
      :description="t('products.empty.categoriesDescription')"
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
      <h2 class="text-h5 mb-4">{{ t("products.cta.title") }}</h2>
      <v-btn color="primary" size="large" to="/products">
        {{ t("common.actions.browseAll") }}
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
