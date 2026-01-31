import { describe, it, expect, beforeEach } from "vitest";
import { createPinia, setActivePinia } from "pinia";
import { useThemeStore } from "./theme";
import { ACTIVE_PALETTE } from "@/theme/themes";

describe("theme store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  describe("initialization", () => {
    it("defaults to light mode", () => {
      const store = useThemeStore();
      expect(store.mode).toBe("light");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);
    });
  });

  describe("setMode", () => {
    it("updates mode and derives themeName", () => {
      const store = useThemeStore();

      store.setMode("dark");

      expect(store.mode).toBe("dark");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-dark`);
    });

    it("can set to light mode", () => {
      const store = useThemeStore();
      store.setMode("dark");

      store.setMode("light");

      expect(store.mode).toBe("light");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);
    });
  });

  describe("toggleMode", () => {
    it("toggles from light to dark", () => {
      const store = useThemeStore();
      expect(store.mode).toBe("light");

      store.toggleMode();

      expect(store.mode).toBe("dark");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-dark`);
    });

    it("toggles from dark to light", () => {
      const store = useThemeStore();
      store.setMode("dark");

      store.toggleMode();

      expect(store.mode).toBe("light");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);
    });

    it("can toggle multiple times", () => {
      const store = useThemeStore();

      store.toggleMode();
      expect(store.mode).toBe("dark");

      store.toggleMode();
      expect(store.mode).toBe("light");

      store.toggleMode();
      expect(store.mode).toBe("dark");
    });
  });

  describe("themeName derivation", () => {
    it("derives theme name from active palette and mode", () => {
      const store = useThemeStore();

      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);

      store.setMode("dark");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-dark`);
    });

    it("always uses the configured ACTIVE_PALETTE", () => {
      const store = useThemeStore();

      expect(store.themeName).toContain(ACTIVE_PALETTE);
      store.toggleMode();
      expect(store.themeName).toContain(ACTIVE_PALETTE);
    });
  });
});
