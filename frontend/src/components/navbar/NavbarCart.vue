<template>
  <v-btn
    icon
    variant="text"
    :aria-label="$t('navbar.cart.label')"
    @click="$emit('click')"
  >
    <v-badge
      v-if="count && count > 0"
      :content="count"
      color="error"
      floating
    >
      <v-icon icon="mdi-cart" />
    </v-badge>
    <v-icon v-else icon="mdi-cart-outline" />
  </v-btn>
</template>

<script setup lang="ts">
defineProps<{
  count?: number;
}>();

defineEmits<{
  click: [];
}>();
</script>
