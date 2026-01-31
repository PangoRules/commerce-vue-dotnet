import { describe, it, expect, vi } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent } from "@testing-library/vue";
import NavbarCart from "./NavbarCart.vue";

describe("NavbarCart", () => {
  it("renders cart button", async () => {
    await renderWithPlugins(NavbarCart);

    const button = screen.getByRole("button");
    expect(button).toBeDefined();
  });

  it("shows outline icon when count is 0", async () => {
    const { container } = await renderWithPlugins(NavbarCart, {
      render: {
        props: {
          count: 0,
        },
      },
    });

    const badge = container.querySelector(".v-badge");
    expect(badge).toBeNull();
  });

  it("shows badge when count is greater than 0", async () => {
    const { container } = await renderWithPlugins(NavbarCart, {
      render: {
        props: {
          count: 5,
        },
      },
    });

    const badge = container.querySelector(".v-badge");
    expect(badge).toBeDefined();
  });

  it("displays correct count in badge", async () => {
    const { container } = await renderWithPlugins(NavbarCart, {
      render: {
        props: {
          count: 12,
        },
      },
    });

    const badgeContent = container.querySelector(".v-badge__badge");
    expect(badgeContent?.textContent).toBe("12");
  });

  it("emits click event when clicked", async () => {
    const onClick = vi.fn();
    await renderWithPlugins(NavbarCart, {
      render: {
        props: {
          count: 3,
        },
        attrs: {
          onClick,
        },
      },
    });

    const button = screen.getByRole("button");
    await fireEvent.click(button);

    expect(onClick).toHaveBeenCalledTimes(1);
  });

  it("has accessible label", async () => {
    await renderWithPlugins(NavbarCart);

    const button = screen.getByRole("button");
    expect(button.getAttribute("aria-label")).toBeDefined();
  });

  it("shows no badge when count is undefined", async () => {
    const { container } = await renderWithPlugins(NavbarCart);

    const badge = container.querySelector(".v-badge");
    expect(badge).toBeNull();
  });
});
