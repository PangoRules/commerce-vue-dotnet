<template>
  <v-app>
    <v-app-bar title="Commerce App" />
    <v-main>
      <v-container class="pa-4">
        <h1>Vue 3 + Vite + Vuetify ✅</h1>
        <v-alert type="success" variant="tonal" class="mt-4">
          Frontend scaffold is running.
        </v-alert>
        <v-alert type="info">{{ statusText }}</v-alert>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { useHealthCheck } from "@/composables/useHealthCheck";
import { computed, onMounted } from "vue";

const { result, check } = useHealthCheck();

const statusText = computed(() => {
  if (!result.value) return "not checked";
  return result.value.ok
    ? result.value.data.status
    : result.value.error.message;
});

onMounted(() => void check());
</script>
