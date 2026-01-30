import { describe, it, expect, beforeEach } from "vitest";
import { createPinia, setActivePinia } from "pinia";
import { useThemeStore } from "./theme";

describe("theme store", () => {
  beforeEach(() => {
    localStorage.clear();
    setActivePinia(createPinia());
  });

  describe("initialization", () => {
    it("defaults to meadow-light when localStorage is empty", () => {
      const store = useThemeStore();
      expect(store.themeName).toBe("meadow-light");
    });

    it("loads theme from localStorage when present", () => {
      localStorage.setItem("app.theme", "sprinkles-dark");
      setActivePinia(createPinia());

      const store = useThemeStore();
      expect(store.themeName).toBe("sprinkles-dark");
    });
  });

  describe("setTheme", () => {
    it("updates themeName state", () => {
      const store = useThemeStore();

      store.setTheme("meadow-dark");

      expect(store.themeName).toBe("meadow-dark");
    });

    it("persists theme to localStorage", () => {
      const store = useThemeStore();

      store.setTheme("sprinkles-light");

      expect(localStorage.getItem("app.theme")).toBe("sprinkles-light");
    });

    it("can switch between all available themes", () => {
      const store = useThemeStore();
      const themes = [
        "meadow-light",
        "meadow-dark",
        "sprinkles-light",
        "sprinkles-dark",
      ] as const;

      themes.forEach((theme) => {
        store.setTheme(theme);
        expect(store.themeName).toBe(theme);
        expect(localStorage.getItem("app.theme")).toBe(theme);
      });
    });
  });

  describe("persistence", () => {
    it("uses app.theme as storage key", () => {
      const store = useThemeStore();

      store.setTheme("meadow-dark");

      expect(localStorage.getItem("app.theme")).toBe("meadow-dark");
      expect(localStorage.getItem("theme")).toBeNull();
    });

    it("survives store recreation", () => {
      const store1 = useThemeStore();
      store1.setTheme("sprinkles-dark");

      setActivePinia(createPinia());
      const store2 = useThemeStore();

      expect(store2.themeName).toBe("sprinkles-dark");
    });
  });
});
