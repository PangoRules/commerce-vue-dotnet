<script setup lang="ts">
defineProps<{
  size?: "small" | "default" | "large";
  text?: string;
}>();
</script>

<template>
  <div class="d-flex flex-column align-center justify-center pa-8">
    <v-progress-circular
      indeterminate
      color="primary"
      :size="size === 'small' ? 24 : size === 'large' ? 64 : 44"
      :width="size === 'small' ? 2 : size === 'large' ? 6 : 4"
    />
    <span v-if="text" class="text-body-2 text-medium-emphasis mt-4">
      {{ text }}
    </span>
  </div>
</template>
