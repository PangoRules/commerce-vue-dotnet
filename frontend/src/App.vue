<script setup lang="ts">
import SnackbarHost from "@/components/shared/SnackbarHost.vue";
import LayoutRenderer from "@/layouts/LayoutRenderer.vue";
import { onMounted, watch } from "vue";
import { useTheme } from "vuetify";
import { useAppStore } from "@/stores/app";

const vuetifyTheme = useTheme();
const appStore = useAppStore();

onMounted(() => {
  // Only set dark mode on first visit (when no persisted preference exists)
  // The store will already have the persisted value if it exists
  if (
    localStorage.getItem("app") === null &&
    window.matchMedia("(prefers-color-scheme: dark)").matches
  ) {
    appStore.setMode("dark");
  }
});

watch(
  () => appStore.themeName,
  (name) => {
    vuetifyTheme.change(name);
  },
  { immediate: true },
);
</script>

<template>
  <v-app>
    <LayoutRenderer />
    <SnackbarHost />
  </v-app>
</template>
