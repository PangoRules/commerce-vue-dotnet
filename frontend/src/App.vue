<template>
  <v-app>
    <v-app-bar>
      <v-app-bar-title>{{ $t("app.name") }}</v-app-bar-title>
    </v-app-bar>

    <v-main>
      <v-container class="pa-4">
        <router-view />
      </v-container>
    </v-main>

    <SnackbarHost />
  </v-app>
</template>

<script setup lang="ts">
import SnackbarHost from "@/components/shared/SnackbarHost.vue";
</script>
