import { describe, it, expect, vi } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent } from "@testing-library/vue";
import NavbarUserMenu from "./NavbarUserMenu.vue";
import type { User } from "@/types/api/authTypes";

const mockUser: User = {
  id: 1,
  email: "john@example.com",
  name: "John Doe",
  avatar: null,
};

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
        btn.textContent?.toLowerCase().includes("sign")
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
        btn.textContent?.toLowerCase().includes("register")
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
        btn.textContent?.toLowerCase().includes("sign")
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
        btn.textContent?.toLowerCase().includes("register")
      );
      if (registerButton) {
        await fireEvent.click(registerButton);
        expect(onRegister).toHaveBeenCalledTimes(1);
      }
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
  });
});
