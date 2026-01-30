<template>
  <v-app>
    <LayoutRenderer />
    <SnackbarHost />
  </v-app>
</template>

<script setup lang="ts">
import SnackbarHost from "@/components/shared/SnackbarHost.vue";
import LayoutRenderer from "@/layouts/LayoutRenderer.vue";
import { watch } from "vue";
import { useTheme } from "vuetify";
import { useThemeStore } from "@/stores/theme";

const vuetifyTheme = useTheme();
const themeStore = useThemeStore();

watch(
  () => themeStore.themeName,
  (name) => {
    vuetifyTheme.global.name.value = name;
  },
  { immediate: true },
);
</script>
