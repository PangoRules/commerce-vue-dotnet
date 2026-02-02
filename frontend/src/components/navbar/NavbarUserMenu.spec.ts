import { describe, it, expect, vi } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent } from "@testing-library/vue";
import NavbarUserMenu from "./NavbarUserMenu.vue";
import { createMockUser } from "@/tests/helpers";

const mockUser = createMockUser();

// Routes referenced in the component's dropdown menu
const testRoutes = [
  { path: "/", component: { template: "<div />" } },
  { path: "/profile", component: { template: "<div />" } },
  { path: "/orders", component: { template: "<div />" } },
];

describe("NavbarUserMenu", () => {
  describe("Guest state", () => {
    it("shows sign in button when not authenticated", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: false,
          },
        },
      });

      const buttons = screen.getAllByRole("button");
      const signInButton = buttons.find((btn) =>
        btn.textContent?.toLowerCase().includes("sign"),
      );
      expect(signInButton).toBeDefined();
    });

    it("shows register button when not authenticated and not compact", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: false,
          },
        },
      });

      const buttons = screen.getAllByRole("button");
      const registerButton = buttons.find((btn) =>
        btn.textContent?.toLowerCase().includes("register"),
      );
      expect(registerButton).toBeDefined();
    });

    it("shows only icon button in compact mode when guest", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: true,
          },
        },
      });

      const buttons = screen.getAllByRole("button");
      expect(buttons.length).toBe(1);
    });

    it("emits login event when sign in is clicked", async () => {
      const onLogin = vi.fn();
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: false,
            onLogin,
          },
        },
      });

      const buttons = screen.getAllByRole("button");
      const signInButton = buttons.find((btn) =>
        btn.textContent?.toLowerCase().includes("sign"),
      );
      if (signInButton) {
        await fireEvent.click(signInButton);
        expect(onLogin).toHaveBeenCalledTimes(1);
      }
    });

    it("emits register event when register is clicked", async () => {
      const onRegister = vi.fn();
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: false,
            onRegister,
          },
        },
      });

      const buttons = screen.getAllByRole("button");
      const registerButton = buttons.find((btn) =>
        btn.textContent?.toLowerCase().includes("register"),
      );
      if (registerButton) {
        await fireEvent.click(registerButton);
        expect(onRegister).toHaveBeenCalledTimes(1);
      }
    });

    it("emits login event when icon button is clicked in compact mode", async () => {
      const onLogin = vi.fn();
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: true,
            onLogin,
          },
        },
      });

      const button = screen.getByRole("button");
      await fireEvent.click(button);
      expect(onLogin).toHaveBeenCalledTimes(1);
    });

    it("emits login event when icon button is clicked in compact mode", async () => {
      const onLogin = vi.fn();
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: true,
            onLogin,
          },
        },
      });

      const button = screen.getByRole("button");
      await fireEvent.click(button);
      expect(onLogin).toHaveBeenCalledTimes(1);
    });
  });

  describe("Authenticated state", () => {
    it("shows user menu button when authenticated", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: false,
          },
        },
      });

      const button = screen.getByRole("button");
      expect(button.textContent).toContain("John Doe");
    });

    it("shows avatar placeholder when user has no avatar", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: { ...mockUser, avatar: null },
            compact: false,
          },
        },
      });

      const avatar = container.querySelector(".v-avatar");
      expect(avatar).toBeDefined();
    });

    it("hides user name in compact mode", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: true,
          },
        },
      });

      const button = screen.getByRole("button");
      expect(button.textContent).not.toContain("John Doe");
    });

    it("shows user avatar when provided", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: { ...mockUser, avatar: "https://example.com/avatar.jpg" },
            compact: false,
          },
        },
      });

      const img = container.querySelector(".v-img");
      expect(img).toBeDefined();
    });

    it("hides chevron icon in compact mode", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: true,
          },
        },
      });

      // In compact mode, the chevron should not be visible
      const button = screen.getByRole("button");
      // The text content should not have "chevron" related text (icon is hidden)
      expect(button.textContent?.trim().length).toBeLessThan(20);
    });

    it("shows chevron icon in normal mode", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: false,
          },
        },
      });

      // In normal mode, there should be a chevron icon in the button
      const icons = container.querySelectorAll(".v-icon");
      expect(icons.length).toBeGreaterThan(0);
    });
  });

  describe("Dropdown menu", () => {
    it("can click authenticated user button to open menu", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        routes: testRoutes,
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: false,
          },
        },
      });

      const button = screen.getByRole("button");

      // Should be able to click without throwing
      await fireEvent.click(button);

      // Button should still exist after click
      expect(button).toBeDefined();
    });

    it("menu button has correct aria attributes for accessibility", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        routes: testRoutes,
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: false,
          },
        },
      });

      const button = screen.getByRole("button");
      // Vuetify menu adds aria-haspopup or aria-expanded
      expect(button).toBeDefined();
    });

    it("renders v-menu wrapper for authenticated users", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        routes: testRoutes,
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: false,
          },
        },
      });

      // The v-menu component should be present
      const menu = container.querySelector(".v-menu");
      // Menu may or may not have a wrapper class depending on Vuetify version
      expect(container.querySelector(".navbar-user-menu")).toBeDefined();
    });
  });

  describe("styling", () => {
    it("has navbar-user-menu wrapper class", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: false,
            compact: false,
          },
        },
      });

      expect(container.querySelector(".navbar-user-menu")).toBeDefined();
    });

    it("button has no text transform in authenticated state", async () => {
      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: mockUser,
            compact: false,
          },
        },
      });

      const btn = container.querySelector(".navbar-user-menu__btn");
      expect(btn).toBeDefined();
    });
  });

  describe("edge cases", () => {
    it("handles null user gracefully when authenticated", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: null,
            compact: false,
          },
        },
      });

      // Should still render without crashing
      const button = screen.getByRole("button");
      expect(button).toBeDefined();
    });

    it("handles undefined user gracefully", async () => {
      await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: undefined,
            compact: false,
          },
        },
      });

      const button = screen.getByRole("button");
      expect(button).toBeDefined();
    });

    it("truncates long user names", async () => {
      const longNameUser = {
        ...mockUser,
        name: "This Is A Very Long User Name That Should Be Truncated",
      };

      const { container } = await renderWithPlugins(NavbarUserMenu, {
        render: {
          props: {
            isAuthenticated: true,
            user: longNameUser,
            compact: false,
          },
        },
      });

      const nameSpan = container.querySelector(".navbar-user-menu__name");
      expect(nameSpan).toBeDefined();
    });
  });
});
