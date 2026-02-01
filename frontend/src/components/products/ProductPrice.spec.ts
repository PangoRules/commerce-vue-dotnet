import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import ProductPrice from "./ProductPrice.vue";

describe("ProductPrice", () => {
  it("formats price with USD currency by default", async () => {
    await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 99.99,
        },
      },
    });

    expect(screen.getByText("$99.99")).toBeDefined();
  });

  it("formats price with specified currency (EUR)", async () => {
    await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 49.5,
          currency: "EUR",
        },
      },
    });

    expect(screen.getByText("€49.50")).toBeDefined();
  });

  it("shows discount when originalPrice > price", async () => {
    await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 79.99,
          originalPrice: 99.99,
        },
      },
    });

    expect(screen.getByText("$79.99")).toBeDefined();
    expect(screen.getByText("$99.99")).toBeDefined();
  });

  it("hides discount when no originalPrice", async () => {
    const { container } = await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 79.99,
        },
      },
    });

    expect(screen.getByText("$79.99")).toBeDefined();
    // Only one price element should be present
    const priceElements = container.querySelectorAll("span");
    // One span for price (no strikethrough span for original price)
    expect(priceElements.length).toBe(1);
  });

  it("hides discount when originalPrice <= price", async () => {
    const { container } = await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 99.99,
          originalPrice: 79.99, // Lower than current price - no discount
        },
      },
    });

    expect(screen.getByText("$99.99")).toBeDefined();
    // Original price should not be shown since it's not greater than current price
    expect(screen.queryByText("$79.99")).toBeNull();
    const priceElements = container.querySelectorAll("span");
    expect(priceElements.length).toBe(1);
  });

  it("applies text-body-2 class for small size", async () => {
    const { container } = await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 50,
          size: "small",
        },
      },
    });

    const priceSpan = container.querySelector(".text-body-2");
    expect(priceSpan).not.toBeNull();
  });

  it("applies text-h6 class for default size", async () => {
    const { container } = await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 50,
        },
      },
    });

    const priceSpan = container.querySelector(".text-h6");
    expect(priceSpan).not.toBeNull();
  });

  it("applies text-h5 class for large size", async () => {
    const { container } = await renderWithPlugins(ProductPrice, {
      render: {
        props: {
          price: 50,
          size: "large",
        },
      },
    });

    const priceSpan = container.querySelector(".text-h5");
    expect(priceSpan).not.toBeNull();
  });
});
