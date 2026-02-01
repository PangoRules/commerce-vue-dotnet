<script setup lang="ts">
import type { User } from "@/types/api/authTypes";

defineProps<{
  isAuthenticated: boolean;
  user?: User | null;
  compact?: boolean;
}>();

defineEmits<{
  login: [];
  logout: [];
  register: [];
}>();
</script>

<template>
  <div class="navbar-user-menu">
    <!-- Authenticated User Menu -->
    <v-menu v-if="isAuthenticated" location="bottom end">
      <template #activator="{ props: menuProps }">
        <v-btn v-bind="menuProps" variant="text" class="navbar-user-menu__btn">
          <v-avatar v-if="user?.avatar" size="32" class="mr-2">
            <v-img :src="user.avatar" :alt="user.name" />
          </v-avatar>
          <v-avatar v-else size="32" color="primary" class="mr-2">
            <v-icon icon="mdi-account" color="white" />
          </v-avatar>
          <span v-if="!compact" class="navbar-user-menu__name">
            {{ user?.name }}
          </span>
          <v-icon v-if="!compact" icon="mdi-chevron-down" size="small" />
        </v-btn>
      </template>
      <v-list density="compact">
        <v-list-item :to="{ path: '/profile' }">
          <template #prepend>
            <v-icon icon="mdi-account-circle" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.profile") }}</v-list-item-title>
        </v-list-item>
        <v-list-item :to="{ path: '/orders' }">
          <template #prepend>
            <v-icon icon="mdi-package-variant-closed" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.orders") }}</v-list-item-title>
        </v-list-item>
        <v-divider class="my-1" />
        <v-list-item @click="$emit('logout')">
          <template #prepend>
            <v-icon icon="mdi-logout" />
          </template>
          <v-list-item-title>{{ $t("navbar.user.signOut") }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>

    <!-- Guest Menu -->
    <template v-else>
      <v-btn v-if="compact" icon variant="text" @click="$emit('login')">
        <v-icon icon="mdi-account" />
      </v-btn>
      <template v-else>
        <v-btn variant="text" @click="$emit('login')">
          {{ $t("navbar.user.signIn") }}
        </v-btn>
        <v-btn variant="outlined" color="primary" @click="$emit('register')">
          {{ $t("navbar.user.register") }}
        </v-btn>
      </template>
    </template>
  </div>
</template>

<style scoped>
.navbar-user-menu {
  display: flex;
  align-items: center;
  gap: 8px;
}

.navbar-user-menu__btn {
  text-transform: none;
}

.navbar-user-menu__name {
  max-width: 120px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
