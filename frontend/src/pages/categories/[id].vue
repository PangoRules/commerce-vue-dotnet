<script setup lang="ts">
import { computed, onMounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useCategories } from "@/composables/useCategories";
import { useProducts } from "@/composables/useProducts";
import type { CategoryAdminDetailsResponse, CategoryResponse } from "@/types/api/categoryTypes";
import type { ProductResponse } from "@/types/api/productTypes";
import type { BreadcrumbItem } from "vuetify/lib/components/VBreadcrumbs/VBreadcrumbs.mjs";
import { useRoute } from "vue-router";
import { LoadingSpinner, EmptyState } from "@/components/shared";
import ProductCarousel from "@/components/products/ProductCarousel.vue";
import { ProductCategorySection } from "@/components/products";

definePage({
  name: "CategoryDetails",
});

const { t } = useI18n();
const route = useRoute<"CategoryDetails">();
const categoryId = computed(() => Number(route.params.id));

const { loadByCategoryId, byCategoryIdResult, isByCategoryIdLoading, loadChildren, childrenResult, isChildrenLoading } =
  useCategories();
const { listProductResult, isProductListLoading, loadProductList } =
  useProducts();
const isLoading = computed(
  () => isByCategoryIdLoading.value || isProductListLoading.value || isChildrenLoading.value,
);
const hasError = computed(
  () =>
    (byCategoryIdResult.value?.ok === false ||
      listProductResult.value?.ok === false) &&
    !isLoading.value,
);

const currentCategory = computed<CategoryAdminDetailsResponse | null>(() => {
  if (!byCategoryIdResult.value?.ok) return null;
  return byCategoryIdResult.value.data;
});
const products = computed<ProductResponse[]>(() => {
  if (!listProductResult.value?.ok) return [];
  return Object.values(listProductResult.value.data);
});
const hasProducts = computed(() => products.value.length > 0);
const childCategories = computed<CategoryResponse[]>(() => {
  if (!childrenResult.value?.ok) return [];
  return childrenResult.value.data;
});
const hasChildCategories = computed(() => childCategories.value.length > 0);

const fetchCategoryData = async () => {
  if (categoryId.value) {
    await loadByCategoryId(categoryId.value);
    await Promise.all([
      loadProductList({ categoryId: categoryId.value, isActive: true }),
      loadChildren(categoryId.value),
    ]);
  }
};
onMounted(fetchCategoryData);
watch(categoryId, fetchCategoryData);

const handleAddToCart = (product: ProductResponse) => {
  // TODO: Implement cart functionality
  console.log("Add to cart:", product);
};

const breadCrumbs = computed<BreadcrumbItem[]>(() => {
  const items: BreadcrumbItem[] = [
    { title: t("common.home"), to: "/", disabled: false },
  ];

  if (!currentCategory.value) return items;

  currentCategory.value.parents.forEach((parent) => {
    items.push({
      title: parent.name,
      to: `/categories/${parent.id}`,
      disabled: false,
    });
  });

  items.push({
    title: currentCategory.value.name,
    to: `/categories/${currentCategory.value.id}`,
    disabled: true,
  });

  return items;
});
</script>

<template>
  <v-container class="py-8 mx-auto">
    <v-breadcrumbs :items="breadCrumbs" class="px-0">
      <template v-slot:divider>
        <v-icon icon="mdi-chevron-right"></v-icon>
      </template>
    </v-breadcrumbs>

    <LoadingSpinner v-if="isLoading" :text="t('categories.loading')" />

    <v-alert v-else-if="hasError" type="error" variant="tonal" class="my-8">
      <template #title>{{ t("categories.failed") }}</template>
      <template #text>
        {{
          (byCategoryIdResult?.ok === false
            ? byCategoryIdResult.error.message
            : null) ||
          (listProductResult?.ok === false
            ? listProductResult.error.message
            : null) ||
          t("errors.unknown")
        }}
      </template>
      <template #append>
        <v-btn variant="text" @click="fetchCategoryData()">
          {{ t("common.actions.retry") }}
        </v-btn>
      </template>
    </v-alert>

    <!-- Category Content Display -->
    <template v-else-if="currentCategory">
      <h1 class="text-h4 font-weight-bold mb-4">
        {{ currentCategory.name }}
      </h1>
      <p v-if="currentCategory.description" class="text-body-1 mb-6">
        {{ currentCategory.description }}
      </p>

      <!-- Direct Products Display -->
      <ProductCarousel
        v-if="hasProducts"
        :products="products"
        @add-to-cart="handleAddToCart"
      />

      <!-- Child Category Subsections -->
      <ProductCategorySection
        v-for="child in childCategories"
        :key="child.id"
        :category="child"
        @add-to-cart="handleAddToCart"
      />

      <!-- Empty State (no direct products and no child categories) -->
      <EmptyState
        v-if="!hasProducts && !hasChildCategories"
        :title="t('products.noProductsYet')"
        :description="
          t('products.noProductsInCategory', { category: currentCategory.name })
        "
        icon="mdi-package-variant-closed"
      />
    </template>

    <!-- No Category Found State (e.g., invalid ID or API error) -->
    <EmptyState
      v-else
      :title="t('categories.notFoundTitle')"
      :description="t('categories.notFoundDescription')"
      icon="mdi-alert-circle-outline"
    />
  </v-container>
</template>
