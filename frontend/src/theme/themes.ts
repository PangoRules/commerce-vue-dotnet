import type { ThemeDefinition } from "vuetify";
import { palettes, type PaletteName } from "@/theme/palettes";

type Mode = "light" | "dark";

const surfaces = {
  light: {
    background: "#F8FAFC",
    surface: "#FFFFFF",
  },
  dark: {
    background: "#0E1512",
    surface: "#16201B",
  },
};

const makeTheme = (name: PaletteName, mode: Mode): ThemeDefinition => {
  const p = palettes[name];
  return {
    dark: mode === "dark",
    colors: {
      primary: p.primary,
      secondary: p.secondary,
      accent: p.accent,

      background: surfaces[mode].background,
      surface: surfaces[mode].surface,

      error: "#EF4444",
      success: "#22C55E",
      warning: "#F59E0B",
      info: "#3B82F6",
    },
  };
};

export const themes = {
  "meadow-light": makeTheme("meadow", "light"),
  "meadow-dark": makeTheme("meadow", "dark"),
  "sprinkles-light": makeTheme("sprinkles", "light"),
  "sprinkles-dark": makeTheme("sprinkles", "dark"),
};

export type ThemeName = keyof typeof themes;
