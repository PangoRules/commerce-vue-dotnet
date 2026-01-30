export interface Palette {
  primary: string;
  secondary: string;
  accent: string;
}

export type PaletteName = "meadow" | "sprinkles";

export const palettes: Record<PaletteName, Palette> = {
  meadow: {
    primary: "#2F855A",
    secondary: "#68D391",
    accent: "#F6AD55",
  },
  sprinkles: {
    primary: "#A78BFA",
    secondary: "#F9A8D4",
    accent: "#7DD3FC",
  },
};
