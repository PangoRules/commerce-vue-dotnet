<script setup lang="ts">
import SnackbarHost from "@/components/shared/SnackbarHost.vue";
import LayoutRenderer from "@/layouts/LayoutRenderer.vue";
import { onMounted, watch } from "vue";
import { useTheme } from "vuetify";
import { useThemeStore } from "@/stores/theme";

const vuetifyTheme = useTheme();
const themeStore = useThemeStore();

onMounted(() => {
  if (window.matchMedia("(prefers-color-scheme: dark)")) {
    themeStore.setMode("dark");
  }
});

watch(
  () => themeStore.themeName,
  (name) => {
    vuetifyTheme.global.name.value = name;
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
