import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import CategoryCard from "./CategoryCard.vue";
import { createMockCategory } from "@/tests/helpers";
import type { ImageAssetResponse } from "@/types/api/imageAssetTypes";

const testRoutes = [
  { path: "/", component: { template: "<div />" } },
  { path: "/categories/:id", component: { template: "<div />" } },
];

function makeImage(id: number): ImageAssetResponse {
  return {
    id: String(id),
    type: "Category",
    ownerId: 1,
    fileName: `image${id}.jpg`,
    contentType: "image/jpeg",
    sizeBytes: 1024,
    displayOrder: id,
    isPrimary: id === 1,
    uploadedAt: "2024-01-01T00:00:00Z",
    url: `https://example.com/image${id}.jpg`,
  };
}

describe("CategoryCard", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders category name", async () => {
    await renderWithPlugins(CategoryCard, {
      render: {
        props: { category: createMockCategory({ name: "Electronics" }) },
      },
      routes: testRoutes,
    });

    expect(screen.getByText("Electronics")).toBeDefined();
  });

  it("renders description when present", async () => {
    await renderWithPlugins(CategoryCard, {
      render: {
        props: {
          category: createMockCategory({
            name: "Clothing",
            description: "All your fashion needs",
          }),
        },
      },
      routes: testRoutes,
    });

    expect(screen.getByText("All your fashion needs")).toBeDefined();
  });

  it("links to /categories/:id", async () => {
    const { container } = await renderWithPlugins(CategoryCard, {
      render: {
        props: { category: createMockCategory({ id: 42, name: "Books" }) },
      },
      routes: testRoutes,
    });

    const link = container.querySelector("a.v-card");
    expect(link).not.toBeNull();
    expect(link?.getAttribute("href")).toBe("/categories/42");
  });

  it("shows image when images array is populated", async () => {
    const { container } = await renderWithPlugins(CategoryCard, {
      render: {
        props: {
          category: createMockCategory({
            images: [makeImage(1)],
          }),
        },
      },
      routes: testRoutes,
    });

    const slideshow = container.querySelector(".category-slideshow");
    expect(slideshow).not.toBeNull();
  });

  it("shows placeholder when images array is empty", async () => {
    const { container } = await renderWithPlugins(CategoryCard, {
      render: {
        props: { category: createMockCategory({ images: [] }) },
      },
      routes: testRoutes,
    });

    const slideshow = container.querySelector(".category-slideshow");
    expect(slideshow).toBeNull();

    // Placeholder img should be present
    const imgs = container.querySelectorAll(".v-img");
    expect(imgs.length).toBeGreaterThan(0);
  });

  it("calls setInterval on mount when multiple images exist", async () => {
    const setIntervalSpy = vi.spyOn(window, "setInterval");

    await renderWithPlugins(CategoryCard, {
      render: {
        props: {
          category: createMockCategory({
            images: [makeImage(1), makeImage(2), makeImage(3)],
          }),
        },
      },
      routes: testRoutes,
    });

    expect(setIntervalSpy).toHaveBeenCalledTimes(1);
    expect(setIntervalSpy).toHaveBeenCalledWith(expect.any(Function), 4000);
  });

  it("does not call setInterval when only one image", async () => {
    const setIntervalSpy = vi.spyOn(window, "setInterval");

    await renderWithPlugins(CategoryCard, {
      render: {
        props: {
          category: createMockCategory({ images: [makeImage(1)] }),
        },
      },
      routes: testRoutes,
    });

    expect(setIntervalSpy).not.toHaveBeenCalled();
  });

  it("does not call setInterval when no images", async () => {
    const setIntervalSpy = vi.spyOn(window, "setInterval");

    await renderWithPlugins(CategoryCard, {
      render: {
        props: { category: createMockCategory({ images: [] }) },
      },
      routes: testRoutes,
    });

    expect(setIntervalSpy).not.toHaveBeenCalled();
  });
});
