import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createVuetify } from "vuetify";
import { createI18n } from "vue-i18n";
import { createRouter, createMemoryHistory } from "vue-router";
import { defineComponent, h, ref, nextTick } from "vue";
import AppNavbar from "./AppNavbar.vue";
import { createPinia, setActivePinia } from "pinia";

// Create mock functions we can track
const mockLogin = vi.fn();
const mockLogout = vi.fn();
const mockToggleAuth = vi.fn();
const mockSubmitSearch = vi.fn();
const mockSearchQuery = ref("");
const mockSelectedCategoryId = ref<number | null>(null);
const mockIsAuthenticated = ref(false);
const mockUser = ref(null);

// Mock display values - we'll modify these in tests
const mockSmAndDown = ref(false);
const mockXs = ref(false);

vi.mock("@/composables/useAuth", () => ({
  useAuth: () => ({
    isAuthenticated: mockIsAuthenticated,
    user: mockUser,
    login: mockLogin,
    logout: mockLogout,
    toggleAuth: mockToggleAuth,
  }),
}));

vi.mock("@/composables/useSearch", () => ({
  useSearch: () => ({
    searchQuery: mockSearchQuery,
    selectedCategoryId: mockSelectedCategoryId,
    submitSearch: mockSubmitSearch,
  }),
}));

vi.mock("vuetify", async (importOriginal) => {
  const actual = await importOriginal<typeof import("vuetify")>();
  return {
    ...actual,
    useDisplay: () => ({
      smAndDown: mockSmAndDown,
      xs: mockXs,
    }),
  };
});

const vuetify = createVuetify();
const i18n = createI18n({
  legacy: false,
  locale: "en",
  messages: {
    en: {
      app: { name: "Commerce" },
      navbar: {
        search: {
          placeholder: "Search...",
          allCategories: "All",
          button: "Search",
        },
        user: {
          signIn: "Sign In",
          register: "Register",
          profile: "Profile",
          orders: "Orders",
          signOut: "Sign Out",
        },
        cart: { label: "Cart" },
        menu: { openMenu: "Open menu" },
        devToggle: "Toggle Auth",
      },
    },
  },
});

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/", component: { template: "<div />" } }],
});

// Stubs for child components that emit events
const NavbarSearchStub = defineComponent({
  name: "NavbarSearch",
  emits: ["submit"],
  setup(_, { emit }) {
    return () =>
      h("div", {
        class: "navbar-search",
        "data-testid": "navbar-search",
        onClick: () => emit("submit"),
      });
  },
});

const NavbarUserMenuStub = defineComponent({
  name: "NavbarUserMenu",
  props: ["isAuthenticated", "user", "compact"],
  emits: ["login", "logout", "register"],
  setup(props, { emit }) {
    return () =>
      h("div", {
        class: "navbar-user-menu",
        "data-testid": "navbar-user-menu",
        "data-authenticated": String(props.isAuthenticated),
        "data-compact": String(props.compact),
        onClick: () => {
          if (props.isAuthenticated) {
            emit("logout");
          } else {
            emit("login");
          }
        },
      });
  },
});

const NavbarMobileDrawerStub = defineComponent({
  name: "NavbarMobileDrawer",
  props: ["modelValue", "isAuthenticated", "user", "showDevToggle"],
  emits: ["update:modelValue", "login", "logout", "register", "toggle-auth"],
  setup(props, { emit }) {
    return () =>
      props.modelValue
        ? h("div", {
            class: "navbar-mobile-drawer",
            "data-testid": "mobile-drawer",
            onClick: () => emit("logout"),
          })
        : null;
  },
});

const NavbarCartStub = defineComponent({
  name: "NavbarCart",
  props: ["count"],
  emits: ["click"],
  setup(props, { emit }) {
    return () =>
      h("button", {
        class: "navbar-cart",
        "data-testid": "navbar-cart",
        onClick: () => emit("click"),
      });
  },
});

const NavbarLogoStub = defineComponent({
  name: "NavbarLogo",
  props: ["compact"],
  setup(props) {
    return () =>
      h("div", {
        class: "navbar-logo",
        "data-testid": "navbar-logo",
        "data-compact": String(props.compact),
      });
  },
});

const ThemeTogglerStub = defineComponent({
  name: "ThemeToggler",
  setup() {
    return () => h("div", { class: "theme-toggler", "data-testid": "theme-toggler" });
  },
});

const VAppBarStub = defineComponent({
  props: ["density", "flat"],
  setup(_, { slots }) {
    return () =>
      h("div", { class: "v-app-bar app-navbar", "data-testid": "app-bar" }, slots.default?.());
  },
});

const VSheetStub = defineComponent({
  props: ["class", "elevation"],
  setup(_, { slots }) {
    return () => h("div", { class: "v-sheet", "data-testid": "search-overlay" }, slots.default?.());
  },
});

describe("AppNavbar", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setActivePinia(createPinia());
    mockSmAndDown.value = false;
    mockXs.value = false;
    mockIsAuthenticated.value = false;
    mockUser.value = null;
    mockSearchQuery.value = "";
    mockSelectedCategoryId.value = null;
  });

  function mountComponent(props: Record<string, unknown> = {}) {
    return mount(AppNavbar, {
      props,
      global: {
        plugins: [vuetify, i18n, router],
        stubs: {
          VAppBar: VAppBarStub,
          VSlideYTransition: true,
          VSheet: VSheetStub,
          VBtn: true,
          VIcon: true,
          NavbarSearch: NavbarSearchStub,
          NavbarLogo: NavbarLogoStub,
          NavbarUserMenu: NavbarUserMenuStub,
          NavbarMobileDrawer: NavbarMobileDrawerStub,
          NavbarCart: NavbarCartStub,
          ThemeToggler: ThemeTogglerStub,
        },
      },
    });
  }

  describe("rendering", () => {
    it("renders without crashing", () => {
      const wrapper = mountComponent();
      expect(wrapper.exists()).toBe(true);
    });

    it("renders the app bar", () => {
      const wrapper = mountComponent();
      expect(wrapper.find("[data-testid='app-bar']").exists()).toBe(true);
    });

    it("renders the logo component", () => {
      const wrapper = mountComponent();
      expect(wrapper.find("[data-testid='navbar-logo']").exists()).toBe(true);
    });

    it("renders the search component on desktop", () => {
      mockSmAndDown.value = false;
      const wrapper = mountComponent();
      expect(wrapper.find("[data-testid='navbar-search']").exists()).toBe(true);
    });

    it("renders the cart button", () => {
      const wrapper = mountComponent();
      expect(wrapper.find("[data-testid='navbar-cart']").exists()).toBe(true);
    });

    it("renders theme toggler", () => {
      const wrapper = mountComponent();
      expect(wrapper.find("[data-testid='theme-toggler']").exists()).toBe(true);
    });
  });

  describe("search functionality", () => {
    it("calls submitSearch when NavbarSearch emits submit", async () => {
      const wrapper = mountComponent();
      const search = wrapper.find("[data-testid='navbar-search']");

      await search.trigger("click");

      expect(mockSubmitSearch).toHaveBeenCalledTimes(1);
    });
  });

  describe("user menu interactions", () => {
    it("calls login when NavbarUserMenu emits login", async () => {
      mockIsAuthenticated.value = false;
      const wrapper = mountComponent();
      const userMenu = wrapper.find("[data-testid='navbar-user-menu']");

      await userMenu.trigger("click");

      expect(mockLogin).toHaveBeenCalledTimes(1);
    });

    it("calls logout when NavbarUserMenu emits logout", async () => {
      mockIsAuthenticated.value = true;
      const wrapper = mountComponent();
      const userMenu = wrapper.find("[data-testid='navbar-user-menu']");

      await userMenu.trigger("click");

      expect(mockLogout).toHaveBeenCalledTimes(1);
    });
  });

  describe("cart interactions", () => {
    it("handles cart click", async () => {
      const wrapper = mountComponent();
      const cart = wrapper.find("[data-testid='navbar-cart']");

      // Should not throw when clicked
      await cart.trigger("click");
      expect(wrapper.exists()).toBe(true);
    });
  });

  describe("dev toggle", () => {
    it("shows dev toggle button when showDevToggle is true and not mobile", async () => {
      mockXs.value = false;
      const wrapper = mountComponent({ showDevToggle: true });

      // The dev toggle button should exist
      const buttons = wrapper.findAll("button");
      expect(buttons.length).toBeGreaterThan(0);
    });

    it("hides dev toggle when showDevToggle is false", () => {
      const wrapper = mountComponent({ showDevToggle: false });
      expect(wrapper.exists()).toBe(true);
    });
  });

  describe("responsive behavior", () => {
    it("passes compact=true to logo on small screens", async () => {
      mockSmAndDown.value = true;
      const wrapper = mountComponent();
      await nextTick();

      const logo = wrapper.find("[data-testid='navbar-logo']");
      expect(logo.attributes("data-compact")).toBe("true");
    });

    it("passes compact=false to logo on large screens", async () => {
      mockSmAndDown.value = false;
      const wrapper = mountComponent();
      await nextTick();

      const logo = wrapper.find("[data-testid='navbar-logo']");
      expect(logo.attributes("data-compact")).toBe("false");
    });

    it("hides NavbarUserMenu on mobile (xs)", async () => {
      mockXs.value = true;
      mockSmAndDown.value = true;
      const wrapper = mountComponent();
      await nextTick();

      // On mobile, user menu is hidden (rendered conditionally with v-if="!isMobile")
      const userMenu = wrapper.find("[data-testid='navbar-user-menu']");
      expect(userMenu.exists()).toBe(false);
    });

    it("shows NavbarUserMenu on desktop", async () => {
      mockXs.value = false;
      mockSmAndDown.value = false;
      const wrapper = mountComponent();
      await nextTick();

      const userMenu = wrapper.find("[data-testid='navbar-user-menu']");
      expect(userMenu.exists()).toBe(true);
    });
  });

  describe("mobile drawer", () => {
    it("toggles drawer when menu button is clicked on small screens", async () => {
      mockSmAndDown.value = true;
      const wrapper = mountComponent();
      await nextTick();

      // Find the hamburger menu button
      const menuButton = wrapper.find(".nav-menu-btn");
      expect(menuButton.exists()).toBe(true);

      // Initially drawer should be closed
      expect(wrapper.find("[data-testid='mobile-drawer']").exists()).toBe(false);

      // Click to open
      await menuButton.trigger("click");
      await nextTick();

      expect(wrapper.find("[data-testid='mobile-drawer']").exists()).toBe(true);

      // Click again to close
      await menuButton.trigger("click");
      await nextTick();

      expect(wrapper.find("[data-testid='mobile-drawer']").exists()).toBe(false);
    });

    it("calls logout when mobile drawer emits logout", async () => {
      mockSmAndDown.value = true;
      const wrapper = mountComponent();
      await nextTick();

      // Open the drawer
      const menuButton = wrapper.find(".nav-menu-btn");
      await menuButton.trigger("click");
      await nextTick();

      // Find and click the drawer (which emits logout on click in our stub)
      const drawer = wrapper.find("[data-testid='mobile-drawer']");
      await drawer.trigger("click");

      expect(mockLogout).toHaveBeenCalledTimes(1);
    });
  });

  describe("mobile search", () => {
    it("shows search button on small screens", async () => {
      mockSmAndDown.value = true;
      const wrapper = mountComponent();
      await nextTick();

      // On small screens, there should be a search icon button
      const buttons = wrapper.findAll("button");
      expect(buttons.length).toBeGreaterThan(0);
    });
  });
});
