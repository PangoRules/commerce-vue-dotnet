<script setup lang="ts">
import type { CategoryResponse } from "@/types/api/categoryTypes";

defineProps<{
  category: CategoryResponse;
  size?: "x-small" | "small" | "default" | "large" | "x-large";
  clickable?: boolean;
}>();
</script>

<template>
  <v-chip
    :size="size ?? 'small'"
    :to="clickable ? `/categories/${category.id}` : undefined"
    color="secondary"
    variant="tonal"
  >
    <v-icon start icon="mdi-tag" size="small" />
    {{ category.name }}
  </v-chip>
</template>
