<script setup lang="ts">
import { computed } from "vue";

const props = defineProps<{
  price: number;
  originalPrice?: number;
  size?: "small" | "default" | "large";
  currency?: string;
}>();

const formattedPrice = computed(() => {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: props.currency ?? "USD",
  }).format(props.price);
});

const formattedOriginalPrice = computed(() => {
  if (!props.originalPrice) return null;
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: props.currency ?? "USD",
  }).format(props.originalPrice);
});

const hasDiscount = computed(() => {
  return props.originalPrice && props.originalPrice > props.price;
});

const textClass = computed(() => {
  switch (props.size) {
    case "small":
      return "text-body-2";
    case "large":
      return "text-h5";
    default:
      return "text-h6";
  }
});
</script>

<template>
  <div class="d-inline-flex align-center ga-2">
    <span :class="[textClass, 'font-weight-bold text-primary']">
      {{ formattedPrice }}
    </span>
    <span
      v-if="hasDiscount"
      class="text-body-2 text-decoration-line-through text-disabled"
    >
      {{ formattedOriginalPrice }}
    </span>
  </div>
</template>
