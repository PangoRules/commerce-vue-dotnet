import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import { createVuetify } from "vuetify";
import { createI18n } from "vue-i18n";
import { defineComponent, h } from "vue";
import NavbarMobileDrawer from "./NavbarMobileDrawer.vue";
import type { User } from "@/types/api/authTypes";

const mockUser: User = {
  id: 1,
  email: "john@example.com",
  name: "John Doe",
  avatar: null,
};

// Create Vuetify instance with layout mocked
const vuetify = createVuetify();
const i18n = createI18n({
  legacy: false,
  locale: "en",
  messages: {
    en: {
      navbar: {
        user: {
          guestGreeting: "Hello, Guest",
          profile: "My Profile",
          orders: "My Orders",
          signIn: "Sign In",
          signOut: "Sign Out",
          register: "Register",
        },
        menu: {
          home: "Home",
          products: "Products",
          categories: "Categories",
        },
        devToggle: "Toggle Auth (Dev)",
      },
    },
  },
});

// Stub the navigation drawer to avoid layout injection issues
const VNavigationDrawerStub = defineComponent({
  props: ["modelValue", "temporary", "location", "width"],
  emits: ["update:modelValue"],
  setup(props, { slots }) {
    return () =>
      props.modelValue
        ? h(
            "div",
            {
              class: "v-navigation-drawer",
              "data-testid": "nav-drawer",
            },
            slots.default?.()
          )
        : null;
  },
});

const RouterLinkStub = defineComponent({
  props: ["to"],
  setup(props, { slots }) {
    return () =>
      h(
        "a",
        { href: typeof props.to === "string" ? props.to : props.to?.path },
        slots.default?.()
      );
  },
});

describe("NavbarMobileDrawer", () => {
  function mountComponent(props: Record<string, unknown> = {}) {
    return mount(NavbarMobileDrawer, {
      props: {
        modelValue: false,
        isAuthenticated: false,
        ...props,
      },
      global: {
        plugins: [vuetify, i18n],
        stubs: {
          VNavigationDrawer: VNavigationDrawerStub,
          RouterLink: RouterLinkStub,
        },
      },
    });
  }

  describe("Guest state", () => {
    it("renders without errors when closed", () => {
      const wrapper = mountComponent({ modelValue: false });
      expect(wrapper.exists()).toBe(true);
    });

    it("shows drawer content when open", () => {
      const wrapper = mountComponent({ modelValue: true });
      const drawer = wrapper.find("[data-testid='nav-drawer']");
      expect(drawer.exists()).toBe(true);
    });

    it("shows guest greeting when not authenticated", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: false,
      });
      expect(wrapper.text()).toContain("Hello, Guest");
    });

    it("shows sign in option", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: false,
      });
      expect(wrapper.text()).toContain("Sign In");
    });

    it("shows register option", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: false,
      });
      expect(wrapper.text()).toContain("Register");
    });
  });

  describe("Authenticated state", () => {
    it("shows user name when authenticated", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: true,
        user: mockUser,
      });
      expect(wrapper.text()).toContain("John Doe");
    });

    it("shows user email when authenticated", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: true,
        user: mockUser,
      });
      expect(wrapper.text()).toContain("john@example.com");
    });

    it("shows sign out option when authenticated", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: true,
        user: mockUser,
      });
      expect(wrapper.text()).toContain("Sign Out");
    });

    it("shows profile link when authenticated", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: true,
        user: mockUser,
      });
      expect(wrapper.text()).toContain("My Profile");
    });

    it("shows orders link when authenticated", () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: true,
        user: mockUser,
      });
      expect(wrapper.text()).toContain("My Orders");
    });
  });

  describe("Navigation links", () => {
    it("shows home link", () => {
      const wrapper = mountComponent({ modelValue: true });
      expect(wrapper.text()).toContain("Home");
    });

    it("shows products link", () => {
      const wrapper = mountComponent({ modelValue: true });
      expect(wrapper.text()).toContain("Products");
    });

    it("shows categories link", () => {
      const wrapper = mountComponent({ modelValue: true });
      expect(wrapper.text()).toContain("Categories");
    });
  });

  describe("Dev toggle", () => {
    it("shows dev toggle when showDevToggle is true", () => {
      const wrapper = mountComponent({
        modelValue: true,
        showDevToggle: true,
      });
      expect(wrapper.text()).toContain("Toggle Auth (Dev)");
    });

    it("hides dev toggle when showDevToggle is false", () => {
      const wrapper = mountComponent({
        modelValue: true,
        showDevToggle: false,
      });
      expect(wrapper.text()).not.toContain("Toggle Auth (Dev)");
    });
  });

  describe("Events", () => {
    it("emits login when sign in is clicked", async () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: false,
      });

      const listItems = wrapper.findAllComponents({ name: "VListItem" });
      const signInItem = listItems.find((item) =>
        item.text().includes("Sign In")
      );

      if (signInItem) {
        await signInItem.trigger("click");
        expect(wrapper.emitted("login")).toBeTruthy();
      }
    });

    it("emits logout when sign out is clicked", async () => {
      const wrapper = mountComponent({
        modelValue: true,
        isAuthenticated: true,
        user: mockUser,
      });

      const listItems = wrapper.findAllComponents({ name: "VListItem" });
      const signOutItem = listItems.find((item) =>
        item.text().includes("Sign Out")
      );

      if (signOutItem) {
        await signOutItem.trigger("click");
        expect(wrapper.emitted("logout")).toBeTruthy();
      }
    });
  });
});
