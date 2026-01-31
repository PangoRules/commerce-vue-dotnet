import { describe, it, expect, vi } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, waitFor } from "@testing-library/vue";
import AddToCartButton from "./AddToCartButton.vue";
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

describe("AddToCartButton", () => {
  it("emits add event with product when clicked", async () => {
    vi.useFakeTimers();
    const product = mockProduct();

    const { emitted } = await renderWithPlugins(AddToCartButton, {
      render: {
        props: {
          product,
        },
      },
    });

    const button = screen.getByRole("button");
    await button.click();

    // Advance past the 300ms delay in the component
    await vi.advanceTimersByTimeAsync(350);

    await waitFor(() => {
      expect(emitted().add).toBeDefined();
    });

    expect(emitted().add[0]).toEqual([product]);

    vi.useRealTimers();
  });

  it("does not emit when stockQuantity is 0", async () => {
    vi.useFakeTimers();
    const product = mockProduct({ stockQuantity: 0 });

    const { emitted } = await renderWithPlugins(AddToCartButton, {
      render: {
        props: {
          product,
        },
      },
    });

    const button = screen.getByRole("button");
    await button.click();

    // Wait a bit to ensure no emit happens
    await vi.advanceTimersByTimeAsync(400);

    expect(emitted().add).toBeUndefined();

    vi.useRealTimers();
  });

  it("shows disabled state when out of stock", async () => {
    const product = mockProduct({ stockQuantity: 0 });

    await renderWithPlugins(AddToCartButton, {
      render: {
        props: {
          product,
        },
      },
    });

    const button = screen.getByRole("button");
    expect(
      button.hasAttribute("disabled") ||
        button.classList.contains("v-btn--disabled")
    ).toBe(true);
  });
});
