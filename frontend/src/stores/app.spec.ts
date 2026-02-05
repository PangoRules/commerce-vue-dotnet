import { describe, it, expect, beforeEach } from "vitest";
import { createPinia, setActivePinia } from "pinia";
import { nextTick } from "vue";
import { useAppStore } from "./app";
import { ACTIVE_PALETTE } from "@/theme/themes";
import { i18n } from "@/i18n";

describe("app store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    // Reset i18n locale to default before each test
    // In vue-i18n v11 with legacy: false, locale is a Ref
    i18n.global.locale.value = "en";
  });

  describe("theme - initialization", () => {
    it("defaults to light mode", () => {
      const store = useAppStore();
      expect(store.mode).toBe("light");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);
    });
  });

  describe("theme - setMode", () => {
    it("updates mode and derives themeName", () => {
      const store = useAppStore();

      store.setMode("dark");

      expect(store.mode).toBe("dark");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-dark`);
    });

    it("can set to light mode", () => {
      const store = useAppStore();
      store.setMode("dark");

      store.setMode("light");

      expect(store.mode).toBe("light");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);
    });
  });

  describe("theme - toggleMode", () => {
    it("toggles from light to dark", () => {
      const store = useAppStore();
      expect(store.mode).toBe("light");

      store.toggleMode();

      expect(store.mode).toBe("dark");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-dark`);
    });

    it("toggles from dark to light", () => {
      const store = useAppStore();
      store.setMode("dark");

      store.toggleMode();

      expect(store.mode).toBe("light");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);
    });

    it("can toggle multiple times", () => {
      const store = useAppStore();

      store.toggleMode();
      expect(store.mode).toBe("dark");

      store.toggleMode();
      expect(store.mode).toBe("light");

      store.toggleMode();
      expect(store.mode).toBe("dark");
    });
  });

  describe("theme - themeName derivation", () => {
    it("derives theme name from active palette and mode", () => {
      const store = useAppStore();

      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-light`);

      store.setMode("dark");
      expect(store.themeName).toBe(`${ACTIVE_PALETTE}-dark`);
    });

    it("always uses the configured ACTIVE_PALETTE", () => {
      const store = useAppStore();

      expect(store.themeName).toContain(ACTIVE_PALETTE);
      store.toggleMode();
      expect(store.themeName).toContain(ACTIVE_PALETTE);
    });
  });

  describe("locale - initialization", () => {
    it("initializes with a valid locale", () => {
      const store = useAppStore();
      expect(["en", "es"]).toContain(store.locale);
    });
  });

  describe("locale - setLocale", () => {
    it("updates locale to a supported value", () => {
      const store = useAppStore();

      store.setLocale("es");

      expect(store.locale).toBe("es");
    });

    it("syncs locale change to vue-i18n", async () => {
      const store = useAppStore();

      store.setLocale("es");
      await nextTick();

      // In vue-i18n v11 with legacy: false, locale is a Ref
      expect(i18n.global.locale.value).toBe("es");
    });

    it("ignores unsupported locale values", () => {
      const store = useAppStore();
      const originalLocale = store.locale;

      store.setLocale("fr" as "en" | "es");

      expect(store.locale).toBe(originalLocale);
    });

    it("can switch between supported locales", () => {
      const store = useAppStore();

      store.setLocale("es");
      expect(store.locale).toBe("es");

      store.setLocale("en");
      expect(store.locale).toBe("en");
    });
  });
});
