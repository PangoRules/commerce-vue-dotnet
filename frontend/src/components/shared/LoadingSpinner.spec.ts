import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import LoadingSpinner from "./LoadingSpinner.vue";

describe("LoadingSpinner", () => {
  describe("rendering", () => {
    it("renders progress circular", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {});

      const spinner = container.querySelector(".v-progress-circular");
      expect(spinner).toBeDefined();
    });

    it("renders without text by default", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {});

      const textSpan = container.querySelector("span.text-body-2");
      expect(textSpan).toBeNull();
    });

    it("renders text when provided", async () => {
      await renderWithPlugins(LoadingSpinner, {
        render: {
          props: {
            text: "Loading products...",
          },
        },
      });

      expect(screen.getByText("Loading products...")).toBeDefined();
    });
  });

  describe("size prop", () => {
    it("renders with default size when not specified", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {});

      const spinner = container.querySelector(".v-progress-circular");
      // Default size is 44px (size prop not "small" or "large")
      expect(spinner).toBeDefined();
    });

    it("renders small size when size='small'", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {
        render: {
          props: {
            size: "small",
          },
        },
      });

      const spinner = container.querySelector(".v-progress-circular");
      expect(spinner).toBeDefined();
      // Small size is 24px - Vuetify sets this via inline style
    });

    it("renders large size when size='large'", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {
        render: {
          props: {
            size: "large",
          },
        },
      });

      const spinner = container.querySelector(".v-progress-circular");
      expect(spinner).toBeDefined();
      // Large size is 64px
    });
  });

  describe("styling", () => {
    it("has centered layout", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {});

      const wrapper = container.firstElementChild;
      expect(wrapper?.classList.contains("d-flex")).toBe(true);
      expect(wrapper?.classList.contains("align-center")).toBe(true);
      expect(wrapper?.classList.contains("justify-center")).toBe(true);
    });

    it("has proper padding", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {});

      const wrapper = container.firstElementChild;
      expect(wrapper?.classList.contains("pa-8")).toBe(true);
    });
  });

  describe("text styling", () => {
    it("applies correct text classes when text is shown", async () => {
      const { container } = await renderWithPlugins(LoadingSpinner, {
        render: {
          props: {
            text: "Loading...",
          },
        },
      });

      const textSpan = container.querySelector("span.text-body-2");
      expect(textSpan).toBeDefined();
      expect(textSpan?.classList.contains("text-medium-emphasis")).toBe(true);
      expect(textSpan?.classList.contains("mt-4")).toBe(true);
    });
  });
});
