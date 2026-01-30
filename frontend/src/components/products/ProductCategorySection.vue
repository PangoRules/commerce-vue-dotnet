<script setup lang="ts">
import { computed, onMounted, watch } from "vue";
import type { CategoryResponse } from "@/types/api/categoryTypes";
import type { ProductResponse } from "@/types/api/productTypes";
import { useProducts } from "@/composables/useProducts";
import ProductCard from "@/components/products/ProductCard.vue";
import ProductCardSkeleton from "@/components/shared/ProductCardSkeleton.vue";
import EmptyState from "@/components/shared/EmptyState.vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  category: CategoryResponse;
  limit?: number;
}>();

const emit = defineEmits<{
  addToCart: [product: ProductResponse];
}>();

const { t } = useI18n();

const { loadProductList, listProductResult, isProductListLoading } =
  useProducts();

const products = computed<ProductResponse[]>(() => {
  if (!listProductResult.value?.ok) return [];
  return Object.values(listProductResult.value.data).slice(0, props.limit ?? 4);
});

const hasProducts = computed(() => products.value.length > 0);
const showViewAll = computed(() => {
  if (!listProductResult.value?.ok) return false;
  return Object.keys(listProductResult.value.data).length > (props.limit ?? 4);
});

const emptyStateDescription = (categoryName: string) => {
  return t("products.noProductsInCategory", {
    category: categoryName,
  });
};

const loadProducts = () => {
  loadProductList({
    categoryId: props.category.id,
    pageSize: (props.limit ?? 4) + 1, // +1 to check if there are more
    isActive: true,
  });
};

onMounted(loadProducts);
watch(() => props.category.id, loadProducts);
</script>

<template>
  <section class="product-category-section my-8">
    <!-- Header -->
    <div class="d-flex justify-space-between align-center mb-4">
      <div>
        <h2 class="text-h5 font-weight-bold">{{ category.name }}</h2>
        <p
          v-if="category.description"
          class="text-body-2 text-medium-emphasis mt-1"
        >
          {{ category.description }}
        </p>
      </div>
      <v-btn
        v-if="showViewAll"
        variant="text"
        color="primary"
        :to="`/categories/${category.id}`"
        append-icon="mdi-arrow-right"
      >
        {{ t("common.actions.viewAll") }}
      </v-btn>
    </div>

    <!-- Loading State -->
    <v-row v-if="isProductListLoading">
      <v-col v-for="i in limit ?? 4" :key="i" cols="12" sm="6" md="3">
        <ProductCardSkeleton />
      </v-col>
    </v-row>

    <!-- Products Grid -->
    <v-row v-else-if="hasProducts">
      <v-col
        v-for="product in products"
        :key="product.id"
        cols="12"
        sm="6"
        md="3"
      >
        <ProductCard
          :product="product"
          @add-to-cart="emit('addToCart', $event)"
        />
      </v-col>
    </v-row>

    <!-- Empty State -->
    <EmptyState
      v-else
      :title="t('products.noProductsYet')"
      :description="emptyStateDescription(category.name)"
      icon="mdi-package-variant-closed"
    />
  </section>
</template>

<style scoped>
.product-category-section {
  border-bottom: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
  padding-bottom: 2rem;
}

.product-category-section:last-child {
  border-bottom: none;
}
</style>
