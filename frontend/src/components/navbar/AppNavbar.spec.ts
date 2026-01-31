import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount } from "@vue/test-utils";
import { createVuetify } from "vuetify";
import { createI18n } from "vue-i18n";
import { createRouter, createMemoryHistory } from "vue-router";
import { defineComponent, h } from "vue";
import AppNavbar from "./AppNavbar.vue";

vi.mock("@/composables/useAuth", () => ({
  useAuth: () => ({
    isAuthenticated: { value: false },
    user: { value: null },
    login: vi.fn(),
    logout: vi.fn(),
    toggleAuth: vi.fn(),
  }),
}));

vi.mock("@/composables/useSearch", () => ({
  useSearch: () => ({
    searchQuery: { value: "" },
    selectedCategoryId: { value: null },
    setSearchQuery: vi.fn(),
    setSelectedCategory: vi.fn(),
    submitSearch: vi.fn(),
  }),
}));

vi.mock("@/composables/useCategories", () => ({
  useCategories: () => ({
    loadCategoryList: vi.fn(),
    listCategoryResult: { value: null },
    isCategoryListLoading: { value: false },
  }),
}));

vi.mock("vuetify", async (importOriginal) => {
  const actual = await importOriginal<typeof import("vuetify")>();
  return {
    ...actual,
    useDisplay: () => ({
      smAndDown: { value: false },
      xs: { value: false },
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
          guestGreeting: "Hello, Guest",
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

// Stub layout-dependent components
const VAppBarStub = defineComponent({
  props: ["density", "flat"],
  setup(_, { slots }) {
    return () =>
      h(
        "div",
        { class: "v-app-bar app-navbar", "data-testid": "app-bar" },
        slots.default?.()
      );
  },
});

const VNavigationDrawerStub = defineComponent({
  props: ["modelValue"],
  setup(props, { slots }) {
    return () => (props.modelValue ? h("div", {}, slots.default?.()) : null);
  },
});

const VSheetStub = defineComponent({
  props: ["class", "elevation"],
  setup(_, { slots }) {
    return () => h("div", { class: "v-sheet" }, slots.default?.());
  },
});

describe("AppNavbar", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  function mountComponent(props: Record<string, unknown> = {}) {
    return mount(AppNavbar, {
      props,
      global: {
        plugins: [vuetify, i18n, router],
        stubs: {
          VAppBar: VAppBarStub,
          VNavigationDrawer: VNavigationDrawerStub,
          VAppBarNavIcon: true,
          VSlideYTransition: true,
          VSheet: VSheetStub,
        },
      },
    });
  }

  it("renders without crashing", () => {
    const wrapper = mountComponent();
    expect(wrapper.exists()).toBe(true);
  });

  it("renders the app bar", () => {
    const wrapper = mountComponent();
    const appBar = wrapper.find("[data-testid='app-bar']");
    expect(appBar.exists()).toBe(true);
  });

  it("renders the logo component", () => {
    const wrapper = mountComponent();
    const logo = wrapper.find(".navbar-logo");
    expect(logo.exists()).toBe(true);
  });

  it("renders the search component", () => {
    const wrapper = mountComponent();
    const search = wrapper.find(".navbar-search");
    expect(search.exists()).toBe(true);
  });

  it("accepts showDevToggle prop", () => {
    const wrapper = mountComponent({ showDevToggle: true });
    expect(wrapper.exists()).toBe(true);
  });

  it("renders cart button", () => {
    const wrapper = mountComponent();
    const buttons = wrapper.findAll("button");
    expect(buttons.length).toBeGreaterThan(0);
  });
});

describe("AppNavbar props", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  function mountComponent(props: Record<string, unknown> = {}) {
    return mount(AppNavbar, {
      props,
      global: {
        plugins: [vuetify, i18n, router],
        stubs: {
          VAppBar: VAppBarStub,
          VNavigationDrawer: VNavigationDrawerStub,
          VAppBarNavIcon: true,
          VSlideYTransition: true,
          VSheet: VSheetStub,
        },
      },
    });
  }

  it("renders with showDevToggle false", () => {
    const wrapper = mountComponent({ showDevToggle: false });
    expect(wrapper.exists()).toBe(true);
  });

  it("renders with showDevToggle true", () => {
    const wrapper = mountComponent({ showDevToggle: true });
    expect(wrapper.exists()).toBe(true);
  });
});
