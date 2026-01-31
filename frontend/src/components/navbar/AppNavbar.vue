<template>
  <v-app-bar density="comfortable" flat class="app-navbar">
    <div class="nav-grid">
      <!-- LEFT -->
      <div class="nav-left">
        <v-app-bar-nav-icon
          v-if="isMobile"
          :aria-label="$t('navbar.menu.openMenu')"
          @click="isDrawerOpen = true"
        />
        <NavbarLogo :compact="isSmall" class="ml-2" />
      </div>

      <!-- CENTER -->
      <div v-if="!isSmall" class="nav-center">
        <NavbarSearch
          v-model="searchQuery"
          v-model:category-id="selectedCategoryId"
          class="nav-search"
          @submit="handleSearch"
        />
      </div>

      <!-- RIGHT -->
      <div class="nav-right">
        <NavbarUserMenu
          v-if="!isMobile"
          :is-authenticated="isAuthenticated"
          :user="user"
          :compact="isSmall"
          @login="handleLogin"
          @logout="handleLogout"
          @register="handleRegister"
        />
        <NavbarCart :count="cartCount" @click="handleCartClick" />
        <ThemeToggler />
        <v-btn
          v-if="showDevToggle && !isMobile"
          icon
          variant="text"
          size="small"
          :title="$t('navbar.devToggle')"
          @click="toggleAuth"
        >
          <v-icon icon="mdi-toggle-switch" size="small" />
        </v-btn>
      </div>
    </div>
  </v-app-bar>

  <!-- Mobile search overlay -->
  <v-slide-y-transition>
    <v-sheet
      v-if="isSearchExpanded && isSmall"
      class="app-navbar__search-overlay pa-2"
      elevation="2"
    >
      <NavbarSearch
        v-model="searchQuery"
        v-model:category-id="selectedCategoryId"
        expanded
        hide-category
        @submit="handleMobileSearch"
      />
    </v-sheet>
  </v-slide-y-transition>

  <!-- Mobile drawer -->
  <NavbarMobileDrawer
    v-model="isDrawerOpen"
    :is-authenticated="isAuthenticated"
    :user="user"
    :show-dev-toggle="showDevToggle"
    @login="handleLogin"
    @logout="handleLogout"
    @register="handleRegister"
    @toggle-auth="toggleAuth"
  />
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useDisplay } from "vuetify";
import { useAuth } from "@/composables/useAuth";
import { useSearch } from "@/composables/useSearch";
import NavbarLogo from "./NavbarLogo.vue";
import NavbarSearch from "./NavbarSearch.vue";
import NavbarUserMenu from "./NavbarUserMenu.vue";
import NavbarCart from "./NavbarCart.vue";
import NavbarMobileDrawer from "./NavbarMobileDrawer.vue";
import ThemeToggler from "@/components/shared/ThemeToggler.vue";

defineProps<{
  showDevToggle?: boolean;
}>();

const { smAndDown, xs } = useDisplay();
const { isAuthenticated, user, login, logout, toggleAuth } = useAuth();
const { searchQuery, selectedCategoryId, submitSearch } = useSearch();

const isDrawerOpen = ref(false);
const isSearchExpanded = ref(false);
const cartCount = ref(0);

const isMobile = computed(() => xs.value);
const isSmall = computed(() => smAndDown.value);

function handleSearch() {
  submitSearch();
}

function handleMobileSearch() {
  submitSearch();
  isSearchExpanded.value = false;
}

function handleLogin() {
  login();
}

function handleLogout() {
  logout();
}

function handleRegister() {
  login();
}

function handleCartClick() {
  // Navigate to cart page (future implementation)
}
</script>

<style scoped>
.nav-grid {
  width: 100%;
  display: grid;
  grid-template-columns: auto minmax(0, 1fr) auto; /* left | center | right */
  align-items: center;
  gap: 1rem; /* tiny space between zones */
}

/* Keep each zone tight */
.nav-left,
.nav-right {
  display: inline-flex;
  align-items: center;
  gap: 8px; /* tiny space between items */
  min-width: 0;
}

/* Center column centers content */
.nav-center {
  display: flex;
  justify-content: center;
  min-width: 0;
}

/* Let search expand but not crush left/right */
.nav-search {
  width: min(720px, 100%);
}
</style>
