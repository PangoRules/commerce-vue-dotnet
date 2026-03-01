<script setup lang="ts">
import { computed } from "vue";

const props = defineProps<{
  price: number;
  salePrice?: number;
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
  if (!props.salePrice) return null;
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: props.currency ?? "USD",
  }).format(props.salePrice);
});

const hasDiscount = computed(() => {
  return props.salePrice != null && props.salePrice < props.price;
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
    <span
      :class="[
        hasDiscount
          ? 'text-disabled text-decoration-line-through'
          : 'text-primary font-weight-bold ' + textClass,
      ]"
    >
      {{ formattedPrice }}
    </span>
    <span
      v-if="hasDiscount"
      :class="['text-primary font-weight-bold', textClass]"
    >
      {{ formattedOriginalPrice }}
    </span>
  </div>
</template>
