import { defineStore } from "pinia";
import { computed, ref, watch } from "vue";
import { getThemeName, type Mode, type ThemeName } from "@/theme/themes";
import { i18n, supportedLocales } from "@/i18n";
import type { Locales } from "@/i18n/i18types";

const defaultLocale: Locales = "en";

function detectBrowserLocale(): Locales {
  const browserLocales = navigator.languages ?? [navigator.language];

  for (const browserLocale of browserLocales) {
    if (!browserLocale) continue;
    const lang = browserLocale.split("-")[0]?.toLowerCase();
    if (supportedLocales.includes(lang as Locales)) {
      return lang as Locales;
    }
  }

  return defaultLocale;
}

export const useAppStore = defineStore(
  "app",
  () => {
    // Theme state
    const mode = ref<Mode>("light");
    const themeName = computed<ThemeName>(() => getThemeName(mode.value));

    function setMode(newMode: Mode) {
      mode.value = newMode;
    }

    function toggleMode() {
      setMode(mode.value === "light" ? "dark" : "light");
    }

    // Locale state - initialize from browser detection
    // (will be overwritten by persisted value if exists)
    const locale = ref<Locales>(detectBrowserLocale());

    function setLocale(newLocale: Locales) {
      if (supportedLocales.includes(newLocale)) {
        locale.value = newLocale;
      }
    }

    // Sync locale changes to vue-i18n
    watch(
      locale,
      (newLocale) => {
        i18n.global.locale = newLocale;
      },
      { immediate: true },
    );

    return {
      // Theme
      mode,
      themeName,
      setMode,
      toggleMode,
      // Locale
      locale,
      setLocale,
    };
  },
  {
    persist: {
      key: "app",
    },
  },
);
