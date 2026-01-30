import { describe, it, expect } from "vitest";
import { palettes, type PaletteName } from "./palettes";

describe("palettes", () => {
  const paletteNames: PaletteName[] = ["meadow", "sprinkles"];

  it("exports all defined palette names", () => {
    paletteNames.forEach((name) => {
      expect(palettes[name]).toBeDefined();
    });
  });

  it.each(paletteNames)("%s palette has all required color keys", (name) => {
    const palette = palettes[name];
    expect(palette).toHaveProperty("primary");
    expect(palette).toHaveProperty("secondary");
    expect(palette).toHaveProperty("accent");
  });

  it.each(paletteNames)("%s palette colors are valid hex values", (name) => {
    const palette = palettes[name];
    const hexPattern = /^#[0-9A-Fa-f]{6}$/;

    expect(palette.primary).toMatch(hexPattern);
    expect(palette.secondary).toMatch(hexPattern);
    expect(palette.accent).toMatch(hexPattern);
  });

  describe("meadow palette", () => {
    it("has expected green-based colors", () => {
      expect(palettes.meadow.primary).toBe("#2F855A");
      expect(palettes.meadow.secondary).toBe("#68D391");
      expect(palettes.meadow.accent).toBe("#F6AD55");
    });
  });

  describe("sprinkles palette", () => {
    it("has expected purple/pink-based colors", () => {
      expect(palettes.sprinkles.primary).toBe("#A78BFA");
      expect(palettes.sprinkles.secondary).toBe("#F9A8D4");
      expect(palettes.sprinkles.accent).toBe("#7DD3FC");
    });
  });
});
