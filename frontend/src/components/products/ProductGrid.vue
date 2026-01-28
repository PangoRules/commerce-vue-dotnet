<script setup lang="ts">
import type { ProductResponse } from "@/types/api/productTypes";
import ProductCard from "./ProductCard.vue";
import ProductCardSkeleton from "@/components/shared/ProductCardSkeleton.vue";
import EmptyState from "@/components/shared/EmptyState.vue";

defineProps<{
  products: ProductResponse[];
  loading?: boolean;
  cols?: {
    xs?: number;
    sm?: number;
    md?: number;
    lg?: number;
    xl?: number;
  };
  skeletonCount?: number;
  showCategory?: boolean;
  showStock?: boolean;
  emptyTitle?: string;
  emptyDescription?: string;
}>();

const emit = defineEmits<{
  addToCart: [product: ProductResponse];
}>();
</script>

<template>
  <!-- Loading State -->
  <v-row v-if="loading">
    <v-col
      v-for="i in (skeletonCount ?? 8)"
      :key="i"
      :cols="cols?.xs ?? 12"
      :sm="cols?.sm ?? 6"
      :md="cols?.md ?? 4"
      :lg="cols?.lg ?? 3"
      :xl="cols?.xl ?? 3"
    >
      <ProductCardSkeleton />
    </v-col>
  </v-row>

  <!-- Products Grid -->
  <v-row v-else-if="products.length > 0">
    <v-col
      v-for="product in products"
      :key="product.id"
      :cols="cols?.xs ?? 12"
      :sm="cols?.sm ?? 6"
      :md="cols?.md ?? 4"
      :lg="cols?.lg ?? 3"
      :xl="cols?.xl ?? 3"
    >
      <ProductCard
        :product="product"
        :show-category="showCategory"
        :show-stock="showStock"
        @add-to-cart="emit('addToCart', $event)"
      />
    </v-col>
  </v-row>

  <!-- Empty State -->
  <EmptyState
    v-else
    :title="emptyTitle ?? 'No products found'"
    :description="emptyDescription ?? 'Try adjusting your filters or search terms'"
    icon="mdi-magnify"
  />
</template>
