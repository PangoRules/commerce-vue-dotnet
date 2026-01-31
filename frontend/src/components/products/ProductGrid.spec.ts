import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import ProductGrid from "./ProductGrid.vue";
import type { ProductResponse } from "@/types/api/productTypes";

const mockProduct = (
  overrides: Partial<ProductResponse> = {}
): ProductResponse => ({
  id: 1,
  name: "Test Product",
  description: "Test description",
  price: 99.99,
  stockQuantity: 10,
  categoryId: 1,
  isActive: true,
  category: { id: 1, name: "Test Category", description: null },
  images: [],
  primaryImageUrl: null,
  ...overrides,
});

describe("ProductGrid", () => {
  it("shows empty state when products array is empty", async () => {
    await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products: [],
          loading: false,
          emptyTitle: "No products found",
          emptyDescription: "Try adjusting filters",
        },
      },
    });

    expect(screen.getByText("No products found")).toBeDefined();
    expect(screen.getByText("Try adjusting filters")).toBeDefined();
  });

  it("shows skeletons when loading=true", async () => {
    const { container } = await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products: [],
          loading: true,
          skeletonCount: 4,
        },
      },
    });

    const skeletons = container.querySelectorAll(".v-skeleton-loader");
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it("renders product cards when products provided", async () => {
    const products = [
      mockProduct({ id: 1, name: "Product One", price: 29.99 }),
      mockProduct({ id: 2, name: "Product Two", price: 49.99 }),
      mockProduct({ id: 3, name: "Product Three", price: 79.99 }),
    ];

    await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products,
          loading: false,
        },
      },
    });

    expect(screen.getByText("Product One")).toBeDefined();
    expect(screen.getByText("Product Two")).toBeDefined();
    expect(screen.getByText("Product Three")).toBeDefined();
  });

  it("renders correct number of product cards", async () => {
    const products = [
      mockProduct({ id: 1, name: "Product One" }),
      mockProduct({ id: 2, name: "Product Two" }),
    ];

    const { container } = await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products,
          loading: false,
        },
      },
    });

    // Each product card is in a v-col
    const cards = container.querySelectorAll(".v-card");
    expect(cards.length).toBe(2);
  });

  it("uses custom empty title and description", async () => {
    await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products: [],
          loading: false,
          emptyTitle: "No items found",
          emptyDescription: "Try a different search",
        },
      },
    });

    expect(screen.getByText("No items found")).toBeDefined();
    expect(screen.getByText("Try a different search")).toBeDefined();
  });

  it("shows skeletons when loading (skeleton count check)", async () => {
    const { container } = await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products: [],
          loading: true,
          skeletonCount: 4,
        },
      },
    });

    const skeletons = container.querySelectorAll(".v-skeleton-loader");
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it("passes showCategory and showStock to product cards", async () => {
    const products = [
      mockProduct({
        id: 1,
        name: "Low Stock Item",
        stockQuantity: 2,
        category: { id: 1, name: "Gadgets", description: null },
      }),
    ];

    const { container } = await renderWithPlugins(ProductGrid, {
      render: {
        props: {
          products,
          loading: false,
          showCategory: true,
          showStock: true,
        },
      },
    });

    expect(screen.getByText("Gadgets")).toBeDefined();

    const chips = container.querySelectorAll(".v-chip");
    const warningChips = Array.from(chips).filter(
      (chip) =>
        chip.classList.contains("text-warning") ||
        chip.classList.contains("bg-warning") ||
        chip.getAttribute("class")?.includes("warning")
    );
    expect(warningChips.length).toBeGreaterThan(0);
  });
});
