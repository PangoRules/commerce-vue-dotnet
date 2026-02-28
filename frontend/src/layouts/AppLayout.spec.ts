import { describe, it, expect, afterEach } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";
import AppLayout from "./AppLayout.vue";

// Stub AppNavbar to check props
const AppNavbarStub = defineComponent({
  name: "AppNavbar",
  props: ["showDevToggle"],
  setup(props) {
    return () =>
      h("div", {
        "data-testid": "app-navbar",
        "data-show-dev-toggle": String(props.showDevToggle),
      });
  },
});

// Stub v-main to allow slot content
const VMainStub = defineComponent({
  name: "VMain",
  setup(_, { slots }) {
    return () => h("main", { "data-testid": "v-main" }, slots.default?.());
  },
});

describe("AppLayout.vue", () => {
  const originalDev = import.meta.env.DEV;

  afterEach(() => {
    (import.meta.env as any).DEV = originalDev;
  });

  function mountComponent(
    options: { isDev?: boolean; slotContent?: string } = {},
  ) {
    if (options.isDev !== undefined) {
      (import.meta.env as any).DEV = options.isDev;
    }

    return mount(AppLayout, {
      global: {
        stubs: {
          AppNavbar: AppNavbarStub,
          "v-main": VMainStub,
        },
      },
      slots: options.slotContent ? { default: options.slotContent } : undefined,
    });
  }

  it("renders AppNavbar component", () => {
    const wrapper = mountComponent();
    const navbar = wrapper.find("[data-testid='app-navbar']");
    expect(navbar.exists()).toBe(true);
  });

  it("passes show-dev-toggle=true when DEV is true", () => {
    const wrapper = mountComponent({ isDev: true });
    const navbar = wrapper.find("[data-testid='app-navbar']");
    expect(navbar.attributes("data-show-dev-toggle")).toBe("true");
  });

  it("passes show-dev-toggle=false when DEV is false", () => {
    const wrapper = mountComponent({ isDev: false });
    const navbar = wrapper.find("[data-testid='app-navbar']");
    expect(navbar.attributes("data-show-dev-toggle")).toBe("false");
  });

  it("renders v-main element", () => {
    const wrapper = mountComponent();
    const main = wrapper.find("[data-testid='v-main']");
    expect(main.exists()).toBe(true);
  });

  it("renders slot content inside v-main", () => {
    const wrapper = mountComponent({
      slotContent: '<div data-testid="slot-content">Hello World</div>',
    });
    const slotContent = wrapper.find("[data-testid='slot-content']");
    expect(slotContent.exists()).toBe(true);
    expect(slotContent.text()).toBe("Hello World");
  });

  it("has app-layout wrapper class", () => {
    const wrapper = mountComponent();
    expect(wrapper.find(".app-layout").exists()).toBe(true);
  });

  it("has app-content wrapper inside v-main", () => {
    const wrapper = mountComponent();
    expect(wrapper.find(".app-content").exists()).toBe(true);
  });
});
