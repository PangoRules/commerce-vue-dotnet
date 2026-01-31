import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen } from "@testing-library/vue";
import NavbarLogo from "./NavbarLogo.vue";

describe("NavbarLogo", () => {
  it("renders app name in non-compact mode", async () => {
    await renderWithPlugins(NavbarLogo, {
      render: {
        props: {
          compact: false,
        },
      },
    });

    const link = screen.getByRole("link");
    expect(link).toBeDefined();
    expect(link.getAttribute("href")).toBe("/");
  });

  it("renders as link to home page", async () => {
    await renderWithPlugins(NavbarLogo);

    const link = screen.getByRole("link");
    expect(link.getAttribute("href")).toBe("/");
  });

  it("applies compact class when compact prop is true", async () => {
    const { container } = await renderWithPlugins(NavbarLogo, {
      render: {
        props: {
          compact: true,
        },
      },
    });

    const logo = container.querySelector(".navbar-logo");
    expect(logo?.classList.contains("compact")).toBe(true);
  });

  it("does not apply compact class when compact is false", async () => {
    const { container } = await renderWithPlugins(NavbarLogo, {
      render: {
        props: {
          compact: false,
        },
      },
    });

    const logo = container.querySelector(".navbar-logo");
    expect(logo?.classList.contains("compact")).toBe(false);
  });

  it("shows icon in compact mode", async () => {
    const { container } = await renderWithPlugins(NavbarLogo, {
      render: {
        props: {
          compact: true,
        },
      },
    });

    const icon = container.querySelector(".v-icon");
    expect(icon).toBeDefined();
  });

  it("shows text in non-compact mode", async () => {
    const { container } = await renderWithPlugins(NavbarLogo, {
      render: {
        props: {
          compact: false,
        },
      },
    });

    const text = container.querySelector(".navbar-logo__text");
    expect(text).toBeDefined();
  });
});
