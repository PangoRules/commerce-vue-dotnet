import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import CategoryChip from "./CategoryChip.vue";
import type { CategoryResponse } from "@/types/api/categoryTypes";

const mockCategory = (
  overrides: Partial<CategoryResponse> = {}
): CategoryResponse => ({
  id: 1,
  name: "Test Category",
  description: null,
  ...overrides,
});

describe("CategoryChip", () => {
  it("renders category name", async () => {
    await renderWithPlugins(CategoryChip, {
      render: {
        props: {
          category: mockCategory({ name: "Electronics" }),
        },
      },
    });

    expect(screen.getByText("Electronics")).toBeDefined();
  });

  it("links to /categories/:id when clickable=true", async () => {
    const { container } = await renderWithPlugins(CategoryChip, {
      render: {
        props: {
          category: mockCategory({ id: 42, name: "Clothing" }),
          clickable: true,
        },
      },
      routes: [
        { path: "/", component: { template: "<div />" } },
        { path: "/categories/:id", component: { template: "<div />" } },
      ],
    });

    // Vuetify v-chip with "to" prop renders as a router-link with an anchor
    const link = container.querySelector("a");
    expect(link).not.toBeNull();
    expect(link?.getAttribute("href")).toBe("/categories/42");
  });

  it("has no link when clickable=false", async () => {
    const { container } = await renderWithPlugins(CategoryChip, {
      render: {
        props: {
          category: mockCategory({ id: 1, name: "Books" }),
          clickable: false,
        },
      },
    });

    // Without clickable, should not render as a link
    const link = container.querySelector("a");
    expect(link).toBeNull();
  });

  it("has no link when clickable is not provided (default)", async () => {
    const { container } = await renderWithPlugins(CategoryChip, {
      render: {
        props: {
          category: mockCategory({ id: 1, name: "Music" }),
        },
      },
    });

    const link = container.querySelector("a");
    expect(link).toBeNull();
  });
});
