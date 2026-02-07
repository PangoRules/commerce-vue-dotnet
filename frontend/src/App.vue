<script setup lang="ts">
import SnackbarHost from "@/components/shared/SnackbarHost.vue";
import LayoutRenderer from "@/layouts/LayoutRenderer.vue";
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { useTheme } from "vuetify";
import { useAppStore } from "@/stores/app";

const vuetifyTheme = useTheme();
const appStore = useAppStore();

onMounted(() => {
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

const appKey = ref(0);
const isSwitchingLocale = ref(false);

// tune these to taste
const FADE_MS = 180;

watch(
  () => appStore.locale,
  async () => {
    isSwitchingLocale.value = true;

    // allow overlay to render first
    await nextTick();

    // remount v-app subtree
    appKey.value += 1;

    // keep overlay briefly to avoid flash
    window.setTimeout(() => {
      isSwitchingLocale.value = false;
    }, FADE_MS);
  },
  { flush: "post" },
);

const vAppKey = computed(() => `${appStore.locale}-${appKey.value}`);
</script>

<template>
  <v-overlay
    :model-value="isSwitchingLocale"
    :scrim="true"
    opacity="0.06"
    :contained="false"
    persistent
  />

  <!-- Vuetify transition around remount -->
  <v-fade-transition mode="out-in">
    <v-app :key="vAppKey">
      <LayoutRenderer />
      <SnackbarHost />
    </v-app>
  </v-fade-transition>
</template>
