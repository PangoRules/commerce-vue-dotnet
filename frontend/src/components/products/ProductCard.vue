<script setup lang="ts">
import { computed } from "vue";
import type { ProductResponse } from "@/types/api/productTypes";
import ProductPrice from "./ProductPrice.vue";
import CategoryChip from "@/components/categories/CategoryChip.vue";

const props = defineProps<{
  product: ProductResponse;
  showCategory?: boolean;
  showStock?: boolean;
}>();

const emit = defineEmits<{
  addToCart: [product: ProductResponse];
}>();

const images = computed(() => props.product.images ?? []);

// Get primary image URL or first image or placeholder
const imageUrl = computed(() => {
  // Check for primaryImageUrl first
  if (props.product.primaryImageUrl) {
    return props.product.primaryImageUrl;
  }
  // Then check images array for primary
  const primaryImage = images.value.find((i) => i.isPrimary);
  if (primaryImage) {
    return primaryImage.url;
  }
  // Then use first image if available
  if (images.value.length > 0 && images.value[0]) {
    return images.value[0].url;
  }
  // Fallback to placeholder
  return "https://placehold.co/400x400/e2e8f0/64748b?text=No+Image";
});

const isLowStock = computed(() => props.product.stockQuantity <= 5);
const isOutOfStock = computed(() => props.product.stockQuantity === 0);

const handleAddToCart = (e: Event) => {
  e.preventDefault();
  e.stopPropagation();
  emit("addToCart", props.product);
};
</script>

<template>
  <v-card hover class="product-card h-100 d-flex flex-column">
    <!-- Image -->
    <div class="position-relative">
      <v-carousel
        v-if="images.length > 1"
        hide-delimiters
        height="100%"
        show-arrows="hover"
      >
        <v-carousel-item v-for="(img, i) in images" :key="i">
          <v-img
            :src="img.url"
            aspect-ratio="1"
            cover
            class="bg-grey-lighten-4"
          >
            <template #placeholder>
              <div class="d-flex align-center justify-center fill-height">
                <v-progress-circular indeterminate color="grey-lighten-2" />
              </div>
            </template>

            <template #error>
              <div
                class="d-flex align-center justify-center fill-height bg-grey-lighten-3"
              >
                <v-icon icon="mdi-image-off" size="48" color="grey-lighten-1" />
              </div>
            </template>
          </v-img>
        </v-carousel-item>
      </v-carousel>

      <!-- Fallback: single image -->
      <v-img
        v-else
        :src="images[0]?.url"
        aspect-ratio="1"
        cover
        class="bg-grey-lighten-4"
      >
        <template #placeholder>
          <div class="d-flex align-center justify-center fill-height">
            <v-progress-circular indeterminate color="grey-lighten-2" />
          </div>
        </template>

        <template #error>
          <div
            class="d-flex align-center justify-center fill-height bg-grey-lighten-3"
          >
            <v-icon icon="mdi-image-off" size="48" color="grey-lighten-1" />
          </div>
        </template>
      </v-img>

      <!-- Out of stock overlay -->
      <div
        v-if="isOutOfStock"
        class="position-absolute w-100 h-100 d-flex align-center justify-center"
        style="top: 0; left: 0; background: rgba(0, 0, 0, 0.5)"
      >
        <v-chip color="error" variant="flat">Out of Stock</v-chip>
      </div>

      <!-- Low stock badge -->
      <v-chip
        v-else-if="isLowStock && showStock"
        color="warning"
        size="small"
        class="position-absolute"
        style="top: 8px; right: 8px"
      >
        Only {{ product.stockQuantity }} left
      </v-chip>
    </div>

    <!-- Content -->
    <v-card-text class="flex-grow-1 d-flex flex-column">
      <!-- Category -->
      <CategoryChip
        v-if="showCategory && product.category"
        :category="product.category"
        size="small"
        class="mb-2"
      />

      <!-- Name -->
      <h3 class="text-subtitle-1 font-weight-medium text-truncate mb-1">
        {{ product.name }}
      </h3>

      <!-- Description -->
      <p
        v-if="product.description"
        class="text-body-2 text-medium-emphasis mb-2 text-truncate-2"
      >
        {{ product.description }}
      </p>

      <v-spacer />

      <!-- Price -->
      <ProductPrice :price="product.price" />
    </v-card-text>

    <!-- Actions -->
    <v-card-actions class="px-4 pb-4">
      <v-btn
        color="primary"
        variant="flat"
        block
        :disabled="isOutOfStock"
        @click="handleAddToCart"
      >
        <v-icon start icon="mdi-cart-plus" />
        Add to Cart
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<style scoped>
.product-card {
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.product-card:hover {
  transform: translateY(-4px);
}

.text-truncate-2 {
  display: -webkit-box;
  line-clamp: 2;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
