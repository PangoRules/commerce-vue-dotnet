<template>
  <router-link to="/" class="navbar-logo" :class="{ compact }">
    <v-icon v-if="compact" icon="mdi-store" size="28" color="primary" />
    <span v-else class="navbar-logo__text text-h6 font-weight-bold">
      {{ $t("app.name") }}
    </span>
  </router-link>
</template>

<script setup lang="ts">
defineProps<{
  compact?: boolean;
}>();
</script>

<style scoped>
.navbar-logo {
  display: flex;
  align-items: center;
  text-decoration: none;
  color: inherit;
}

.navbar-logo__text {
  white-space: nowrap;
}

.navbar-logo.compact {
  padding: 4px;
}
</style>
