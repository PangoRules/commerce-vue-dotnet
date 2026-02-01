import { describe, it, expect, vi } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent } from "@testing-library/vue";
import EmptyState from "./EmptyState.vue";

describe("EmptyState", () => {
  describe("rendering", () => {
    it("renders the title", async () => {
      await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items found",
          },
        },
      });

      expect(screen.getByText("No items found")).toBeDefined();
    });

    it("renders the description when provided", async () => {
      await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items found",
            description: "Try adjusting your search criteria",
          },
        },
      });

      expect(screen.getByText("Try adjusting your search criteria")).toBeDefined();
    });

    it("does not render description when not provided", async () => {
      await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items found",
          },
        },
      });

      const paragraphs = screen.queryAllByRole("paragraph");
      const descriptionP = paragraphs.find(
        (p) => p.classList.contains("text-body-2")
      );
      expect(descriptionP).toBeUndefined();
    });

    it("renders custom icon when provided", async () => {
      const { container } = await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items",
            icon: "mdi-cart-off",
          },
        },
      });

      const icon = container.querySelector(".v-icon");
      expect(icon).toBeDefined();
    });

    it("renders default icon when not provided", async () => {
      const { container } = await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items",
          },
        },
      });

      const icon = container.querySelector(".v-icon");
      expect(icon).toBeDefined();
    });
  });

  describe("action button", () => {
    it("renders action button when actionText is provided", async () => {
      await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items",
            actionText: "Add Item",
          },
        },
      });

      expect(screen.getByRole("button", { name: "Add Item" })).toBeDefined();
    });

    it("does not render action button when actionText is not provided", async () => {
      await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items",
          },
        },
      });

      expect(screen.queryByRole("button")).toBeNull();
    });

    it("emits action event when button is clicked", async () => {
      const onAction = vi.fn();
      await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items",
            actionText: "Add Item",
            onAction,
          },
        },
      });

      const button = screen.getByRole("button", { name: "Add Item" });
      await fireEvent.click(button);

      expect(onAction).toHaveBeenCalledTimes(1);
    });
  });

  describe("styling", () => {
    it("has centered layout", async () => {
      const { container } = await renderWithPlugins(EmptyState, {
        render: {
          props: {
            title: "No items",
          },
        },
      });

      const wrapper = container.firstElementChild;
      expect(wrapper?.classList.contains("d-flex")).toBe(true);
      expect(wrapper?.classList.contains("align-center")).toBe(true);
      expect(wrapper?.classList.contains("justify-center")).toBe(true);
    });
  });
});
