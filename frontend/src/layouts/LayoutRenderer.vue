<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import {
  layoutMap,
  DEFAULT_LAYOUT,
  type LayoutKey,
  layoutKeys,
} from "@/layouts/layoutTypes";

const route = useRoute();

const layoutKey = computed<LayoutKey>(() => {
  const key = route.meta.layout as LayoutKey | undefined;
  return key ?? DEFAULT_LAYOUT;
});

const LayoutComponent = computed(() => {
  const key = layoutKey.value;

  // extra-safety: fallback even if someone bypassed TS
  if (import.meta.env.DEV && !layoutKeys.includes(key)) {
    console.warn(
      `[layout] Unknown layout "${String(route.meta.layout)}", falling back to "${DEFAULT_LAYOUT}"`,
    );
    return layoutMap[DEFAULT_LAYOUT];
  }

  return layoutMap[key] ?? layoutMap[DEFAULT_LAYOUT];
});
</script>

<template>
  <component :is="LayoutComponent">
    <RouterView />
  </component>
</template>
