<template>
  <div class="app-layout">
    <AppNavbar :show-dev-toggle="isDev" />

    <v-main>
      <div class="app-content">
        <slot />
      </div>
    </v-main>
  </div>
</template>

<script setup lang="ts">
import { AppNavbar } from "@/components/navbar";

const isDev = import.meta.env.DEV;
</script>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-content {
  flex: 1;
  min-height: 0; /* IMPORTANT: enables proper scrolling */
  display: flex;
  flex-direction: column;
}
</style>
