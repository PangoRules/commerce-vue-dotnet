<script setup lang="ts">
</script>

<template>
  <v-card>
    <v-skeleton-loader type="image" height="200" />
    <v-card-text>
      <v-skeleton-loader type="text" class="mb-2" />
      <v-skeleton-loader type="text" width="60%" />
    </v-card-text>
    <v-card-actions>
      <v-skeleton-loader type="button" width="100%" />
    </v-card-actions>
  </v-card>
</template>
