<template>
  <v-app>
    <LayoutRenderer />
    <SnackbarHost />
  </v-app>
</template>

<script setup lang="ts">
import SnackbarHost from "@/components/shared/SnackbarHost.vue";
import LayoutRenderer from "@/layouts/LayoutRenderer.vue";
</script>
