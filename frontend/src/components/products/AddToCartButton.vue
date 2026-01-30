<script setup lang="ts">
import { ref } from "vue";
import type { ProductResponse } from "@/types/api/productTypes";

const props = defineProps<{
  product: ProductResponse;
  block?: boolean;
  size?: "x-small" | "small" | "default" | "large" | "x-large";
}>();

const emit = defineEmits<{
  add: [product: ProductResponse];
}>();

const isAdding = ref(false);
const justAdded = ref(false);

const handleClick = async () => {
  if (props.product.stockQuantity === 0) return;

  isAdding.value = true;

  // Simulate async action (replace with actual cart logic later)
  await new Promise((resolve) => setTimeout(resolve, 300));

  emit("add", props.product);

  isAdding.value = false;
  justAdded.value = true;

  // Reset the "added" state after animation
  setTimeout(() => {
    justAdded.value = false;
  }, 1500);
};
</script>

<template>
  <v-btn
    :color="justAdded ? 'success' : 'primary'"
    :variant="justAdded ? 'tonal' : 'flat'"
    :block="block"
    :size="size"
    :disabled="product.stockQuantity === 0"
    :loading="isAdding"
    @click="handleClick"
  >
    <v-icon start :icon="justAdded ? 'mdi-check' : 'mdi-cart-plus'" />
    {{ justAdded ? 'Added!' : product.stockQuantity === 0 ? 'Out of Stock' : 'Add to Cart' }}
  </v-btn>
</template>
