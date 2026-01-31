<script setup lang="ts">
import { useToastStore } from "@/stores/toast";
const toast = useToastStore();
</script>

<template>
  <v-snackbar
    v-model="toast.show"
    :timeout="toast.timeout"
    :color="toast.level"
  >
    {{ toast.text }}
    <template #actions>
      <v-btn variant="text" @click="toast.show = false">Close</v-btn>
    </template>
  </v-snackbar>
</template>
