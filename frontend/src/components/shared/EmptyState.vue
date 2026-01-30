<script setup lang="ts">
defineProps<{
  icon?: string;
  title: string;
  description?: string;
  actionText?: string;
}>();

const emit = defineEmits<{
  action: [];
}>();
</script>

<template>
  <div class="d-flex flex-column align-center justify-center pa-8 text-center">
    <v-icon
      :icon="icon ?? 'mdi-package-variant'"
      size="64"
      color="grey-lighten-1"
      class="mb-4"
    />
    <h3 class="text-h6 text-medium-emphasis mb-2">{{ title }}</h3>
    <p v-if="description" class="text-body-2 text-disabled mb-4">
      {{ description }}
    </p>
    <v-btn
      v-if="actionText"
      color="primary"
      variant="tonal"
      @click="emit('action')"
    >
      {{ actionText }}
    </v-btn>
  </div>
</template>
