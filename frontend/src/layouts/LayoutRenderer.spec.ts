import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, reactive, nextTick } from "vue";

// Reactive route mock so computed updates work
const route = reactive<{ meta: Record<string, any> }>({ meta: {} });

// Mock useRoute only (don’t try to mock RouterView here — we’ll stub it in mount)
vi.mock("vue-router", () => ({
  useRoute: () => route,
}));

// Mock layoutTypes to match your current reality (only "app")
vi.mock("@/layouts/layoutTypes", () => {
  const AppLayout = defineComponent({
    name: "AppLayout",
    template: "<div data-test='layout-app'><slot /></div>",
  });

  return {
    layoutKeys: ["app"],
    DEFAULT_LAYOUT: "app",
    layoutMap: { app: AppLayout },
  };
});

import LayoutRenderer from "@/layouts/LayoutRenderer.vue";

const RouterViewStub = defineComponent({
  name: "RouterView",
  template: "<div data-test='router-view' />",
});

function mountRenderer() {
  return mount(LayoutRenderer, {
    global: {
      // KEY: this prevents the “Failed to resolve component: RouterView” Vue warn
      stubs: {
        RouterView: RouterViewStub,
      },
    },
  });
}

describe("LayoutRenderer.vue (single layout: app)", () => {
  const originalDev = import.meta.env.DEV;

  beforeEach(() => {
    route.meta = {};
    vi.restoreAllMocks();
  });

  afterEach(() => {
    (import.meta.env as any).DEV = originalDev;
  });

  it("renders RouterView inside the default app layout when meta.layout is missing", async () => {
    const wrapper = mountRenderer();
    await nextTick();

    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);
    expect(wrapper.find("[data-test='router-view']").exists()).toBe(true);
  });

  it("renders app layout when meta.layout is explicitly 'app'", async () => {
    route.meta.layout = "app";

    const wrapper = mountRenderer();
    await nextTick();

    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);
    expect(wrapper.find("[data-test='router-view']").exists()).toBe(true);
  });

  it("in DEV: warns (our warning) and falls back when meta.layout is unknown", async () => {
    (import.meta.env as any).DEV = true;

    route.meta.layout = "not-a-real-layout";

    const warn = vi.spyOn(console, "warn").mockImplementation(() => {});
    const wrapper = mountRenderer();
    await nextTick();

    // filter to ONLY your layout warning (ignore any framework warnings)
    const layoutWarns = warn.mock.calls
      .map((c) => String(c[0]))
      .filter((msg) => msg.startsWith("[layout] Unknown layout"));

    expect(layoutWarns.length).toBeGreaterThanOrEqual(1);
    expect(layoutWarns[0]).toContain('Unknown layout "not-a-real-layout"');
    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);
  });

  it("in PROD: does NOT emit our layout warning, but still falls back when meta.layout is unknown", async () => {
    (import.meta.env as any).DEV = false;

    route.meta.layout = "not-a-real-layout";

    const warn = vi.spyOn(console, "warn").mockImplementation(() => {});
    const wrapper = mountRenderer();
    await nextTick();

    const layoutWarns = warn.mock.calls
      .map((c) => String(c[0]))
      .filter((msg) => msg.startsWith("[layout] Unknown layout"));

    expect(layoutWarns.length).toBe(0);
    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);
  });

  it("reacts when route.meta.layout changes at runtime", async () => {
    (import.meta.env as any).DEV = false;

    const wrapper = mountRenderer();
    await nextTick();

    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);

    route.meta.layout = "wat";
    await nextTick();
    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);

    route.meta.layout = "app";
    await nextTick();
    expect(wrapper.find("[data-test='layout-app']").exists()).toBe(true);
  });
});
