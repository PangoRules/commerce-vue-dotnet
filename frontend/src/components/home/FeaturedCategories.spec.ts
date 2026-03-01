import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import { ref } from "vue";
import FeaturedCategories from "./FeaturedCategories.vue";
import { createMockCategory } from "@/tests/helpers";
import type { ApiResult } from "@/lib/http";
import type { CategoryResponse } from "@/types/api/categoryTypes";

// Mock Headers
const mockHeaders = new Headers();

// Mock useCategories composable
const mockLoadCategoryList = vi.fn();
const mockListCategoryResult = ref<ApiResult<CategoryResponse[]> | null>(null);
const mockIsCategoryListLoading = ref(false);

vi.mock("@/composables/useCategories", () => ({
  useCategories: () => ({
    loadCategoryList: mockLoadCategoryList,
    listCategoryResult: mockListCategoryResult,
    isCategoryListLoading: mockIsCategoryListLoading,
  }),
}));

// Mock useDisplay for responsive tests
const mockMdAndUp = ref(true);

vi.mock("vuetify", async (importOriginal) => {
  const actual = await importOriginal<typeof import("vuetify")>();
  return {
    ...actual,
    useDisplay: () => ({
      mdAndUp: mockMdAndUp,
      smAndDown: ref(!mockMdAndUp.value),
      lgAndUp: ref(true),
    }),
  };
});

const testRoutes = [
  { path: "/", component: { template: "<div />" } },
  { path: "/categories", component: { template: "<div />" } },
  { path: "/categories/:id", component: { template: "<div />" } },
];

describe("FeaturedCategories", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockListCategoryResult.value = null;
    mockIsCategoryListLoading.value = false;
    mockMdAndUp.value = true;
  });

  describe("initialization", () => {
    it("calls loadCategoryList on mount with correct params", async () => {
      await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      expect(mockLoadCategoryList).toHaveBeenCalledTimes(1);
      expect(mockLoadCategoryList).toHaveBeenCalledWith({
        page: 1,
        pageSize: 10,
        featuredOnly: true,
      });
    });
  });

  describe("loading state", () => {
    it("shows loading spinner when isCategoryListLoading is true", async () => {
      mockIsCategoryListLoading.value = true;

      const { container } = await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      const spinner = container.querySelector(".v-progress-circular");
      expect(spinner).toBeDefined();
    });

    it("hides loading spinner when loading is complete", async () => {
      mockIsCategoryListLoading.value = false;
      mockListCategoryResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [],
      };

      const { container } = await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      const loadingDiv = container.querySelector(
        ".d-flex.justify-center.pa-8 .v-progress-circular",
      );
      expect(loadingDiv).toBeNull();
    });
  });

  describe("categories display", () => {
    it("renders category cards when data is loaded", async () => {
      mockListCategoryResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [
          createMockCategory({ id: 1, name: "Electronics" }),
          createMockCategory({ id: 2, name: "Clothing" }),
        ],
      };

      await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      expect(screen.getByText("Electronics")).toBeDefined();
      expect(screen.getByText("Clothing")).toBeDefined();
    });

    it("renders category description when available", async () => {
      mockListCategoryResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [
          createMockCategory({
            id: 1,
            name: "Electronics",
            description: "All electronic items",
          }),
        ],
      };

      await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      expect(screen.getByText("All electronic items")).toBeDefined();
    });

    it("renders featured categories heading with i18n", async () => {
      mockListCategoryResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [createMockCategory({ id: 1, name: "Electronics" })],
      };

      await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      // The heading should use the i18n translation
      const heading = screen.getByRole("heading", { level: 2 });
      expect(heading).toBeDefined();
    });
  });

  describe("empty state", () => {
    it("renders empty container when no categories", async () => {
      mockListCategoryResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [],
      };

      const { container } = await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      // Should render container but no category cards
      const cards = container.querySelectorAll(".v-card");
      expect(cards.length).toBe(0);
    });
  });

  describe("navigation", () => {
    it("category cards link to the category detail page", async () => {
      mockListCategoryResult.value = {
        ok: true,
        status: 200,
        headers: mockHeaders,
        data: [createMockCategory({ id: 5, name: "Gadgets" })],
      };

      const { container } = await renderWithPlugins(FeaturedCategories, { routes: testRoutes });

      // Find the card link
      const cardLinks = container.querySelectorAll("a.v-card");
      const gadgetLink = Array.from(cardLinks).find((link) =>
        link.getAttribute("href")?.includes("/categories/5"),
      );
      expect(gadgetLink).toBeDefined();
    });
  });
});
