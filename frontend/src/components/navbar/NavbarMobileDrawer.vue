<template>
  <v-navigation-drawer
    v-model="isOpen"
    temporary
    location="left"
    width="280"
  >
    <v-list density="compact" nav>
      <!-- User Section -->
      <v-list-item v-if="isAuthenticated && user" class="mb-2">
        <template #prepend>
          <v-avatar v-if="user.avatar" size="40">
            <v-img :src="user.avatar" :alt="user.name" />
          </v-avatar>
          <v-avatar v-else size="40" color="primary">
            <v-icon icon="mdi-account" color="white" />
          </v-avatar>
        </template>
        <v-list-item-title class="font-weight-medium">
          {{ user.name }}
        </v-list-item-title>
        <v-list-item-subtitle>{{ user.email }}</v-list-item-subtitle>
      </v-list-item>
      <v-list-item v-else class="mb-2">
        <template #prepend>
          <v-avatar size="40" color="grey-lighten-2">
            <v-icon icon="mdi-account" />
          </v-avatar>
        </template>
        <v-list-item-title>{{ $t("navbar.user.guestGreeting") }}</v-list-item-title>
      </v-list-item>

      <v-divider class="mb-2" />

      <!-- Navigation Links -->
      <v-list-item :to="{ path: '/' }" @click="close">
        <template #prepend>
          <v-icon icon="mdi-home" />
        </template>
        <v-list-item-title>{{ $t("navbar.menu.home") }}</v-list-item-title>
      </v-list-item>

      <v-list-item :to="{ path: '/products' }" @click="close">
        <template #prepend>
          <v-icon icon="mdi-view-grid" />
        </template>
        <v-list-item-title>{{ $t("navbar.menu.products") }}</v-list-item-title>
      </v-list-item>

      <v-list-item :to="{ path: '/categories' }" @click="close">
        <template #prepend>
          <v-icon icon="mdi-tag" />
        </template>
        <v-list-item-title>{{ $t("navbar.menu.categories") }}</v-list-item-title>
      </v-list-item>

      <template v-if="isAuthenticated">
        <v-divider class="my-2" />

        <v-list-item :to="{ path: '/profile' }" @click="close">
          <template #prepend>
            <v-icon icon="mdi-account-circle" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.profile") }}</v-list-item-title>
        </v-list-item>

        <v-list-item :to="{ path: '/orders' }" @click="close">
          <template #prepend>
            <v-icon icon="mdi-package-variant-closed" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.orders") }}</v-list-item-title>
        </v-list-item>

        <v-divider class="my-2" />

        <v-list-item @click="handleLogout">
          <template #prepend>
            <v-icon icon="mdi-logout" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.signOut") }}</v-list-item-title>
        </v-list-item>
      </template>

      <template v-else>
        <v-divider class="my-2" />

        <v-list-item @click="handleLogin">
          <template #prepend>
            <v-icon icon="mdi-login" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.signIn") }}</v-list-item-title>
        </v-list-item>

        <v-list-item @click="handleRegister">
          <template #prepend>
            <v-icon icon="mdi-account-plus" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.register") }}</v-list-item-title>
        </v-list-item>
      </template>

      <!-- Dev toggle for testing -->
      <template v-if="showDevToggle">
        <v-divider class="my-2" />
        <v-list-item @click="$emit('toggleAuth')">
          <template #prepend>
            <v-icon icon="mdi-toggle-switch" />
          </template>
          <v-list-item-title>{{ $t("navbar.devToggle") }}</v-list-item-title>
        </v-list-item>
      </template>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { computed } from "vue";
import type { User } from "@/types/api/authTypes";

const props = defineProps<{
  modelValue: boolean;
  isAuthenticated: boolean;
  user?: User | null;
  showDevToggle?: boolean;
}>();

const emit = defineEmits<{
  "update:modelValue": [value: boolean];
  login: [];
  logout: [];
  register: [];
  toggleAuth: [];
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (value) => emit("update:modelValue", value),
});

function close() {
  isOpen.value = false;
}

function handleLogin() {
  emit("login");
  close();
}

function handleLogout() {
  emit("logout");
  close();
}

function handleRegister() {
  emit("register");
  close();
}
</script>
