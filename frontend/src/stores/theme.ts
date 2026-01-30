import { defineStore } from "pinia";
import type { ThemeName } from "@/theme/themes";

const storageKey = "app.theme";

export const useThemeStore = defineStore("theme", {
  state: () => ({
    themeName: (localStorage.getItem(storageKey) ??
      "meadow-light") as ThemeName,
  }),
  actions: {
    setTheme(theme: ThemeName) {
      this.themeName = theme;
      localStorage.setItem(storageKey, theme);
    },
  },
});
