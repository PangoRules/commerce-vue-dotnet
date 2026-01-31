import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import ProductCard from "./ProductCard.vue";
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

describe("ProductCard", () => {
  it("shows low stock badge when stockQuantity <= 5 and showStock=true", async () => {
    const { container } = await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({ stockQuantity: 3 }),
          showStock: true,
        },
      },
    });

    // Vuetify v-chip with color="warning" adds text-warning class or similar
    const chips = container.querySelectorAll(".v-chip");
    const warningChips = Array.from(chips).filter(
      (chip) =>
        chip.classList.contains("text-warning") ||
        chip.classList.contains("bg-warning") ||
        chip.getAttribute("class")?.includes("warning")
    );
    expect(warningChips.length).toBeGreaterThan(0);
  });

  it("hides low stock badge when showStock=false", async () => {
    const { container } = await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({ stockQuantity: 3 }),
          showStock: false,
        },
      },
    });

    const chips = container.querySelectorAll(".v-chip");
    const warningChips = Array.from(chips).filter(
      (chip) =>
        chip.classList.contains("text-warning") ||
        chip.classList.contains("bg-warning") ||
        chip.getAttribute("class")?.includes("warning")
    );
    expect(warningChips.length).toBe(0);
  });

  it("shows out of stock overlay when stockQuantity === 0", async () => {
    const { container } = await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({ stockQuantity: 0 }),
        },
      },
    });

    const chips = container.querySelectorAll(".v-chip");
    const errorChips = Array.from(chips).filter(
      (chip) =>
        chip.classList.contains("text-error") ||
        chip.classList.contains("bg-error") ||
        chip.getAttribute("class")?.includes("error")
    );
    expect(errorChips.length).toBeGreaterThan(0);
  });

  it("renders without images gracefully", async () => {
    await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({ images: [], primaryImageUrl: null }),
        },
      },
    });

    expect(screen.getByText("Test Product")).toBeDefined();
    expect(screen.getByText("$99.99")).toBeDefined();
  });

  it("renders product name and description", async () => {
    await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({
            name: "Awesome Widget",
            description: "A great product for everyone",
          }),
        },
      },
    });

    expect(screen.getByText("Awesome Widget")).toBeDefined();
    expect(screen.getByText("A great product for everyone")).toBeDefined();
  });

  it("includes AddToCartButton component", async () => {
    await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct(),
        },
      },
    });

    // The AddToCartButton renders a button
    const button = screen.getByRole("button");
    expect(button).toBeDefined();
  });

  it("shows category chip when showCategory=true and category exists", async () => {
    await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({
            category: { id: 5, name: "Electronics", description: null },
          }),
          showCategory: true,
        },
      },
    });

    expect(screen.getByText("Electronics")).toBeDefined();
  });

  it("hides category chip when showCategory=false", async () => {
    await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({
            category: { id: 5, name: "Electronics", description: null },
          }),
          showCategory: false,
        },
      },
    });

    expect(screen.queryByText("Electronics")).toBeNull();
  });

  it("does not show low stock badge when stockQuantity > 5", async () => {
    const { container } = await renderWithPlugins(ProductCard, {
      render: {
        props: {
          product: mockProduct({ stockQuantity: 10 }),
          showStock: true,
        },
      },
    });

    const chips = container.querySelectorAll(".v-chip");
    const warningChips = Array.from(chips).filter(
      (chip) =>
        chip.classList.contains("text-warning") ||
        chip.classList.contains("bg-warning") ||
        chip.getAttribute("class")?.includes("warning")
    );
    expect(warningChips.length).toBe(0);
  });
});
