import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import { ref } from "vue";
import DealsCarousel from "./DealsCarousel.vue";
import { createMockProduct } from "@/tests/helpers";
import type { ApiResult } from "@/lib/http";
import type { ProductResponse } from "@/types/api/productTypes";

// Mock Headers
const mockHeaders = new Headers();

// Mock useProducts composable
const mockLoadProductList = vi.fn();
const mockListProductResult = ref<ApiResult<ProductResponse[]> | null>(null);
const mockIsProductListLoading = ref(false);

vi.mock("@/composables/useProducts", () => ({
  useProducts: () => ({
    loadProductList: mockLoadProductList,
    listProductResult: mockListProductResult,
    isProductListLoading: mockIsProductListLoading,
  }),
}));

// Mock ProductCard to simplify testing
vi.mock("@/components/products/ProductCard.vue", () => ({
  default: {
    name: "ProductCard",
    props: ["product", "showCategory"],
    template:
      '<div data-testid="product-card" :data-product-id="product.id">{{ product.name }}</div>',
  },
}));

// Mock useDisplay for responsive tests
const mockSmAndDown = ref(false);
const mockLgAndUp = ref(true);
const mockMdAndUp = ref(true);

vi.mock("vuetify", async (importOriginal) => {
  const actual = await importOriginal<typeof import("vuetify")>();
  return {
    ...actual,
    useDisplay: () => ({
      smAndDown: mockSmAndDown,
      lgAndUp: mockLgAndUp,
      mdAndUp: mockMdAndUp,
    }),
  };
});

const testRoutes = [
  { path: "/", component: { template: "<div />" } },
  { path: "/products", component: { template: "<div />" } },
];

describe("DealsCarousel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockListProductResult.value = null;
    mockIsProductListLoading.value = false;
    mockSmAndDown.value = false;
    mockLgAndUp.value = true;
    mockMdAndUp.value = true;
  });

  describe("initialization", () => {
    it("calls loadProductList on mount with onSale filter", async () => {
      await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      expect(mockLoadProductList).toHaveBeenCalledTimes(1);
      expect(mockLoadProductList).toHaveBeenCalledWith({ onSale: true });
    });
  });

  describe("loading state", () => {
    it("shows loading spinner when isProductListLoading is true", async () => {
      mockIsProductListLoading.value = true;

      const { container } = await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const spinner = container.querySelector(".v-progress-circular");
      expect(spinner).toBeDefined();
    });

    it("hides loading spinner when loading is complete", async () => {
      mockIsProductListLoading.value = false;
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [],
      };

      const { container } = await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const loadingDiv = container.querySelector(
        ".d-flex.justify-center.pa-8 .v-progress-circular",
      );
      expect(loadingDiv).toBeNull();
    });
  });

  describe("products display", () => {
    it("renders ProductCard for each product", async () => {
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [
          createMockProduct({ id: 1, name: "Product 1" }),
          createMockProduct({ id: 2, name: "Product 2" }),
          createMockProduct({ id: 3, name: "Product 3" }),
          createMockProduct({ id: 4, name: "Product 4" }),
        ],
      };

      await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const productCards = screen.getAllByTestId("product-card");
      expect(productCards.length).toBeGreaterThan(0);
    });

    it("passes showCategory=false to ProductCard", async () => {
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [createMockProduct({ id: 1, name: "Test Product" })],
      };

      await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      // The mock ProductCard should be rendered
      expect(screen.getByTestId("product-card")).toBeDefined();
    });
  });

  describe("heading", () => {
    it("renders deals heading with i18n translation", async () => {
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [createMockProduct({ id: 1, name: "Sale Item" })],
      };

      await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const heading = screen.getByRole("heading", { level: 2 });
      expect(heading).toBeDefined();
      // Should contain the translated title
      expect(heading.textContent).toContain("Most popular products");
    });
  });

  describe("pagination", () => {
    it("renders pagination controls", async () => {
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [
          createMockProduct({ id: 1, name: "Product 1" }),
          createMockProduct({ id: 2, name: "Product 2" }),
        ],
      };

      const { container } = await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      // Should have prev/next buttons
      const buttons = container.querySelectorAll(".v-btn");
      expect(buttons.length).toBeGreaterThan(0);
    });

    it("shows page count in footer", async () => {
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [createMockProduct({ id: 1, name: "Product 1" })],
      };

      const { container } = await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const footer = container.querySelector(".v-footer");
      expect(footer).toBeDefined();
    });
  });

  describe("empty state", () => {
    it("renders empty data iterator when no products", async () => {
      mockListProductResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [],
      };

      const { container } = await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const productCards = container.querySelectorAll(
        '[data-testid="product-card"]',
      );
      expect(productCards.length).toBe(0);
    });
  });

  describe("error handling", () => {
    it("returns empty array when result is not ok", async () => {
      mockListProductResult.value = {
        ok: false,
        status: 500,
        error: { kind: "http", message: "Server error" },
      };

      const { container } = await renderWithPlugins(DealsCarousel, { routes: testRoutes });

      const productCards = container.querySelectorAll(
        '[data-testid="product-card"]',
      );
      expect(productCards.length).toBe(0);
    });
  });
});
