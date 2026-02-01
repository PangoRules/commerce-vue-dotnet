import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";
import { createVuetify } from "vuetify";
import { createI18n } from "vue-i18n";
import { ref } from "vue";
import ProductCarousel from "./ProductCarousel.vue";
import { createMockProduct } from "@/tests/helpers";

// Mock useDisplay from Vuetify
vi.mock("vuetify", async (importOriginal) => {
  const actual = await importOriginal<typeof import("vuetify")>();
  return {
    ...actual,
    useDisplay: () => ({
      mdAndUp: ref(true),
      smAndDown: ref(false),
      xs: ref(false),
    }),
  };
});

// Stub ProductCard
const ProductCardStub = defineComponent({
  name: "ProductCard",
  props: ["product", "showCategory"],
  setup(props) {
    return () =>
      h("div", {
        class: "product-card-stub",
        "data-testid": `product-card-${props.product.id}`,
        "data-show-category": String(props.showCategory),
      }, props.product.name);
  },
});

// Stub Vuetify components
const VSheetStub = defineComponent({
  name: "VSheet",
  props: ["class", "elevation"],
  setup(_, { slots }) {
    return () => h("div", { class: "v-sheet" }, slots.default?.());
  },
});

const VSlideGroupStub = defineComponent({
  name: "VSlideGroup",
  props: ["class", "showArrows"],
  setup(props, { slots }) {
    return () =>
      h(
        "div",
        {
          class: "v-slide-group",
          "data-show-arrows": String(props.showArrows),
        },
        slots.default?.()
      );
  },
});

const VSlideGroupItemStub = defineComponent({
  name: "VSlideGroupItem",
  setup(_, { slots }) {
    return () => h("div", { class: "v-slide-group-item" }, slots.default?.({}));
  },
});

const vuetify = createVuetify();
const i18n = createI18n({
  legacy: false,
  locale: "en",
  messages: { en: {} },
});

describe("ProductCarousel", () => {
  function mountComponent(products = [createMockProduct()]) {
    return mount(ProductCarousel, {
      props: { products },
      global: {
        plugins: [vuetify, i18n],
        stubs: {
          ProductCard: ProductCardStub,
          VSheet: VSheetStub,
          VSlideGroup: VSlideGroupStub,
          VSlideGroupItem: VSlideGroupItemStub,
        },
      },
    });
  }

  describe("rendering", () => {
    it("renders without crashing", () => {
      const wrapper = mountComponent();
      expect(wrapper.exists()).toBe(true);
    });

    it("renders VSheet wrapper", () => {
      const wrapper = mountComponent();
      expect(wrapper.find(".v-sheet").exists()).toBe(true);
    });

    it("renders VSlideGroup", () => {
      const wrapper = mountComponent();
      expect(wrapper.find(".v-slide-group").exists()).toBe(true);
    });
  });

  describe("products", () => {
    it("renders a ProductCard for each product", () => {
      const products = [
        createMockProduct({ id: 1, name: "Product 1" }),
        createMockProduct({ id: 2, name: "Product 2" }),
        createMockProduct({ id: 3, name: "Product 3" }),
      ];

      const wrapper = mountComponent(products);
      const cards = wrapper.findAll(".product-card-stub");

      expect(cards.length).toBe(3);
    });

    it("passes correct product to each ProductCard", () => {
      const products = [
        createMockProduct({ id: 1, name: "First Product" }),
        createMockProduct({ id: 2, name: "Second Product" }),
      ];

      const wrapper = mountComponent(products);

      expect(wrapper.find("[data-testid='product-card-1']").text()).toBe("First Product");
      expect(wrapper.find("[data-testid='product-card-2']").text()).toBe("Second Product");
    });

    it("passes showCategory=false to ProductCard", () => {
      const wrapper = mountComponent([createMockProduct({ id: 1 })]);
      const card = wrapper.find("[data-testid='product-card-1']");

      expect(card.attributes("data-show-category")).toBe("false");
    });

    it("renders empty when no products provided", () => {
      const wrapper = mountComponent([]);
      const cards = wrapper.findAll(".product-card-stub");

      expect(cards.length).toBe(0);
    });
  });

  describe("responsive behavior", () => {
    it("shows arrows on larger screens (mdAndUp)", () => {
      const wrapper = mountComponent();
      const slideGroup = wrapper.find(".v-slide-group");

      // mdAndUp is mocked as true
      expect(slideGroup.attributes("data-show-arrows")).toBe("true");
    });
  });

  describe("structure", () => {
    it("wraps each ProductCard in a slide group item", () => {
      const products = [
        createMockProduct({ id: 1 }),
        createMockProduct({ id: 2 }),
      ];

      const wrapper = mountComponent(products);
      const slideGroupItems = wrapper.findAll(".v-slide-group-item");

      expect(slideGroupItems.length).toBe(2);
    });
  });
});
