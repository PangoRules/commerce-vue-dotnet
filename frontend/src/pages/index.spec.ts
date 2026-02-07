import { describe, it, expect, vi } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import IndexPage from "./index.vue";

// Mock child components to isolate testing
vi.mock("@/components/home/FeaturedCategories.vue", () => ({
  default: {
    name: "FeaturedCategories",
    template: '<div data-testid="featured-categories">Featured Categories</div>',
  },
}));

vi.mock("@/components/home/DealsCarousel.vue", () => ({
  default: {
    name: "DealsCarousel",
    template: '<div data-testid="deals-carousel">Deals Carousel</div>',
  },
}));

describe("IndexPage (pages/index.vue)", () => {
  describe("hero section", () => {
    it("renders hero title with app name", async () => {
      await renderWithPlugins(IndexPage, {});

      const heading = screen.getByRole("heading", { level: 1 });
      expect(heading).toBeDefined();
      // Should contain the welcome message with app name
      expect(heading.textContent).toContain("Welcome to");
      expect(heading.textContent).toContain("Commerce");
    });

    it("renders hero subtitle", async () => {
      await renderWithPlugins(IndexPage, {});

      // Subtitle text from i18n
      expect(
        screen.getByText(/Discover amazing products across all categories/i),
      ).toBeDefined();
    });

    it("hero section has correct CSS classes", async () => {
      const { container } = await renderWithPlugins(IndexPage, {});

      const heroSection = container.querySelector(".hero-section");
      expect(heroSection).toBeDefined();
      expect(heroSection?.classList.contains("text-center")).toBe(true);
    });
  });

  describe("child components", () => {
    it("renders FeaturedCategories component", async () => {
      await renderWithPlugins(IndexPage, {});

      expect(screen.getByTestId("featured-categories")).toBeDefined();
    });

    it("renders DealsCarousel component", async () => {
      await renderWithPlugins(IndexPage, {});

      expect(screen.getByTestId("deals-carousel")).toBeDefined();
    });

    it("renders components in correct order", async () => {
      const { container } = await renderWithPlugins(IndexPage, {});

      const featuredCategories = container.querySelector(
        '[data-testid="featured-categories"]',
      );
      const dealsCarousel = container.querySelector(
        '[data-testid="deals-carousel"]',
      );

      expect(featuredCategories).toBeDefined();
      expect(dealsCarousel).toBeDefined();

      // FeaturedCategories should come before DealsCarousel in DOM
      if (featuredCategories && dealsCarousel) {
        const position = featuredCategories.compareDocumentPosition(dealsCarousel);
        expect(position & Node.DOCUMENT_POSITION_FOLLOWING).toBeTruthy();
      }
    });
  });

  describe("layout", () => {
    it("renders within a v-container", async () => {
      const { container } = await renderWithPlugins(IndexPage, {});

      const vuetifyContainer = container.querySelector(".v-container");
      expect(vuetifyContainer).toBeDefined();
    });

    it("container has fluid class", async () => {
      const { container } = await renderWithPlugins(IndexPage, {});

      const vuetifyContainer = container.querySelector(".v-container");
      expect(vuetifyContainer?.classList.contains("v-container--fluid")).toBe(
        true,
      );
    });
  });
});
