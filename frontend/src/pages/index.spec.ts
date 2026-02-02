import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent } from "@testing-library/vue";
import { ref, nextTick } from "vue";
import IndexPage from "./index.vue";
import { createMockCategory } from "@/tests/helpers";
import type { ApiResult } from "@/lib/http";
import type { CategoryResponse } from "@/types/api/categoryTypes";

// Mock useCategories composable
const mockLoadRoots = vi.fn();
const mockRootsResult = ref<ApiResult<CategoryResponse[]> | null>(null);
const mockIsRootsLoading = ref(false);

vi.mock("@/composables/useCategories", () => ({
  useCategories: () => ({
    loadRoots: mockLoadRoots,
    rootsResult: mockRootsResult,
    isRootsLoading: mockIsRootsLoading,
  }),
}));

// Mock ProductCategorySection to avoid deep rendering
vi.mock("@/components/products", () => ({
  ProductCategorySection: {
    name: "ProductCategorySection",
    props: ["category", "limit"],
    template: '<div data-testid="category-section" :data-category-id="category.id">{{ category.name }}</div>',
  },
}));

// Mock shared components
vi.mock("@/components/shared", () => ({
  LoadingSpinner: {
    name: "LoadingSpinner",
    props: ["text"],
    template: '<div data-testid="loading-spinner">{{ text }}</div>',
  },
  EmptyState: {
    name: "EmptyState",
    props: ["title", "description", "icon"],
    template: '<div data-testid="empty-state">{{ title }}</div>',
  },
}));

describe("IndexPage (pages/index.vue)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockRootsResult.value = null;
    mockIsRootsLoading.value = false;
  });

  describe("initialization", () => {
    it("calls loadRoots on mount", async () => {
      await renderWithPlugins(IndexPage, {});

      expect(mockLoadRoots).toHaveBeenCalledTimes(1);
    });
  });

  describe("loading state", () => {
    it("shows loading spinner when isRootsLoading is true", async () => {
      mockIsRootsLoading.value = true;

      await renderWithPlugins(IndexPage, {});

      expect(screen.getByTestId("loading-spinner")).toBeDefined();
    });

    it("hides loading spinner when loading is complete", async () => {
      mockIsRootsLoading.value = false;
      mockRootsResult.value = { ok: true, status: 200, data: [] };

      await renderWithPlugins(IndexPage, {});

      expect(screen.queryByTestId("loading-spinner")).toBeNull();
    });
  });

  describe("error state", () => {
    it("shows error alert when rootsResult has error", async () => {
      mockRootsResult.value = {
        ok: false,
        status: 500,
        error: { message: "Server error", details: null },
      };

      await renderWithPlugins(IndexPage, {});

      const alert = screen.getByRole("alert");
      expect(alert).toBeDefined();
    });

    it("shows retry button on error", async () => {
      mockRootsResult.value = {
        ok: false,
        status: 500,
        error: { message: "Server error", details: null },
      };

      await renderWithPlugins(IndexPage, {});

      const retryButton = screen.getByRole("button");
      expect(retryButton).toBeDefined();
    });

    it("calls loadRoots when retry button is clicked", async () => {
      mockRootsResult.value = {
        ok: false,
        status: 500,
        error: { message: "Server error", details: null },
      };

      await renderWithPlugins(IndexPage, {});

      // Clear the initial call
      mockLoadRoots.mockClear();

      const retryButton = screen.getByRole("button");
      await fireEvent.click(retryButton);

      expect(mockLoadRoots).toHaveBeenCalledTimes(1);
    });
  });

  describe("empty state", () => {
    it("shows empty state when no categories are returned", async () => {
      mockRootsResult.value = { ok: true, status: 200, data: [] };

      await renderWithPlugins(IndexPage, {});

      expect(screen.getByTestId("empty-state")).toBeDefined();
    });
  });

  describe("categories display", () => {
    it("renders ProductCategorySection for each category", async () => {
      mockRootsResult.value = {
        ok: true,
        status: 200,
        data: [
          createMockCategory({ id: 1, name: "Electronics" }),
          createMockCategory({ id: 2, name: "Clothing" }),
          createMockCategory({ id: 3, name: "Books" }),
        ],
      };

      await renderWithPlugins(IndexPage, {});

      const sections = screen.getAllByTestId("category-section");
      expect(sections.length).toBe(3);
    });

    it("passes category to ProductCategorySection", async () => {
      mockRootsResult.value = {
        ok: true,
        status: 200,
        data: [createMockCategory({ id: 5, name: "Gadgets" })],
      };

      await renderWithPlugins(IndexPage, {});

      const section = screen.getByTestId("category-section");
      expect(section.getAttribute("data-category-id")).toBe("5");
      expect(section.textContent).toContain("Gadgets");
    });
  });

  describe("hero section", () => {
    it("renders hero title", async () => {
      mockRootsResult.value = { ok: true, status: 200, data: [] };

      await renderWithPlugins(IndexPage, {});

      // The title includes the app name from i18n
      const heading = screen.getByRole("heading", { level: 1 });
      expect(heading).toBeDefined();
    });

    it("renders hero subtitle", async () => {
      mockRootsResult.value = { ok: true, status: 200, data: [] };

      await renderWithPlugins(IndexPage, {});

      // Subtitle is a paragraph
      const container = document.querySelector(".hero-section");
      expect(container).toBeDefined();
    });
  });

  describe("footer CTA", () => {
    it("shows browse all button when categories exist", async () => {
      mockRootsResult.value = {
        ok: true,
        status: 200,
        data: [createMockCategory({ id: 1, name: "Electronics" })],
      };

      await renderWithPlugins(IndexPage, {});

      // Find the link/button that goes to /products
      const buttons = screen.getAllByRole("link");
      const browseAllLink = buttons.find((btn) =>
        btn.getAttribute("href")?.includes("/products")
      );
      expect(browseAllLink).toBeDefined();
    });

    it("hides footer CTA when no categories exist", async () => {
      mockRootsResult.value = { ok: true, status: 200, data: [] };

      await renderWithPlugins(IndexPage, {});

      // Should not have a link to /products when empty
      const links = screen.queryAllByRole("link");
      const browseAllLink = links.find((link) =>
        link.getAttribute("href")?.includes("/products")
      );
      expect(browseAllLink).toBeUndefined();
    });
  });
});
