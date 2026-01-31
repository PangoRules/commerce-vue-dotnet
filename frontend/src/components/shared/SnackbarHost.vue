<script setup lang="ts">
import { useToastStore } from "@/stores/toast";
import { useI18n } from "vue-i18n";
const toast = useToastStore();
const { t } = useI18n();
</script>

<template>
  <v-snackbar
    v-model="toast.show"
    :timeout="toast.timeout"
    :color="toast.level"
  >
    {{ toast.text }}
    <template #actions>
      <v-btn variant="text" @click="toast.show = false">{{
        t("common.actions.close")
      }}</v-btn>
    </template>
  </v-snackbar>
</template>
