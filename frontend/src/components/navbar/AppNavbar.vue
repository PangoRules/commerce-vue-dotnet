<template>
  <v-app-bar density="comfortable" flat class="app-navbar">
    <div class="nav-grid">
      <!-- LEFT -->
      <div class="nav-left">
        <button
          v-if="isSmall"
          type="button"
          class="nav-menu-btn"
          :class="{ 'nav-menu-btn--active': isDrawerOpen }"
          :aria-label="$t('navbar.menu.openMenu')"
          :aria-expanded="isDrawerOpen"
          @click="isDrawerOpen = !isDrawerOpen"
        >
          <span class="nav-menu-btn__bar" />
          <span class="nav-menu-btn__bar" />
          <span class="nav-menu-btn__bar" />
        </button>
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
        <v-btn
          v-if="isSmall"
          icon
          variant="text"
          size="default"
          :aria-label="$t('navbar.search.button')"
          @click="isSearchExpanded = !isSearchExpanded"
        >
          <v-icon icon="mdi-magnify" />
        </v-btn>
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
import { ref, computed, watch } from "vue";
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

watch(isSmall, (value) => {
  if (!value) {
    isSearchExpanded.value = false;
  }
});
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

.nav-left {
  grid-column: 1;
}

.nav-menu-btn {
  width: 40px;
  height: 40px;
  display: inline-flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 5px;
  padding: 0;
  background: transparent;
  border: 0;
  border-radius: 8px;
  color: inherit;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.nav-menu-btn:focus-visible {
  outline: 2px solid var(--v-theme-primary);
  outline-offset: 2px;
}

.nav-menu-btn__bar {
  width: 18px;
  height: 2px;
  background: currentColor;
  border-radius: 2px;
  transition:
    transform 0.25s ease,
    opacity 0.2s ease;
}

.nav-menu-btn--active .nav-menu-btn__bar:nth-child(1) {
  transform: translateY(7px) rotate(45deg);
}

.nav-menu-btn--active .nav-menu-btn__bar:nth-child(2) {
  opacity: 0;
}

.nav-menu-btn--active .nav-menu-btn__bar:nth-child(3) {
  transform: translateY(-7px) rotate(-45deg);
}

/* Center column centers content */
.nav-center {
  display: flex;
  justify-content: center;
  min-width: 0;
  grid-column: 2;
}

/* Let search expand but not crush left/right */
.nav-search {
  width: min(720px, 100%);
}

.nav-right {
  grid-column: 3;
  justify-self: end;
}

.app-navbar__search-overlay {
  position: fixed;
  top: var(--v-layout-top, 64px);
  left: 0;
  right: 0;
  z-index: calc(var(--v-layout-z-index, 2000) + 1);
}
</style>
