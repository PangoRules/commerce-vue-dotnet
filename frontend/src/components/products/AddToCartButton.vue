<script setup lang="ts">
import { ref } from "vue";
import type { ProductResponse } from "@/types/api/productTypes";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

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
    <span class="d-inline-block text-truncate" style="max-width: 100px">{{
      justAdded
        ? t("products.cart.added")
        : product.stockQuantity === 0
          ? t("products.outOfStock")
          : t("products.cart.add")
    }}</span>
  </v-btn>
</template>
