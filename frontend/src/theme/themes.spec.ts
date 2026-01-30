import { describe, it, expect } from "vitest";
import { themes, type ThemeName } from "./themes";

describe("themes", () => {
  const themeNames: ThemeName[] = [
    "meadow-light",
    "meadow-dark",
    "sprinkles-light",
    "sprinkles-dark",
  ];

  it("exports all expected theme names", () => {
    themeNames.forEach((name) => {
      expect(themes[name]).toBeDefined();
    });
  });

  it("has exactly 4 themes", () => {
    expect(Object.keys(themes)).toHaveLength(4);
  });

  describe.each(themeNames)("%s theme", (themeName) => {
    const theme = themes[themeName];

    it("has dark property matching mode", () => {
      const expectedDark = themeName.endsWith("-dark");
      expect(theme.dark).toBe(expectedDark);
    });

    it("has required palette colors", () => {
      expect(theme.colors).toHaveProperty("primary");
      expect(theme.colors).toHaveProperty("secondary");
      expect(theme.colors).toHaveProperty("accent");
    });

    it("has surface colors", () => {
      expect(theme.colors).toHaveProperty("background");
      expect(theme.colors).toHaveProperty("surface");
    });

    it("has status colors", () => {
      expect(theme.colors).toHaveProperty("error");
      expect(theme.colors).toHaveProperty("success");
      expect(theme.colors).toHaveProperty("warning");
      expect(theme.colors).toHaveProperty("info");
    });

    it("has valid hex colors", () => {
      const hexPattern = /^#[0-9A-Fa-f]{6}$/;
      const colors = theme.colors!;

      expect(colors.primary).toMatch(hexPattern);
      expect(colors.secondary).toMatch(hexPattern);
      expect(colors.accent).toMatch(hexPattern);
      expect(colors.background).toMatch(hexPattern);
      expect(colors.surface).toMatch(hexPattern);
      expect(colors.error).toMatch(hexPattern);
      expect(colors.success).toMatch(hexPattern);
      expect(colors.warning).toMatch(hexPattern);
      expect(colors.info).toMatch(hexPattern);
    });
  });

  describe("light themes", () => {
    it("have light surface colors", () => {
      expect(themes["meadow-light"].colors?.background).toBe("#F8FAFC");
      expect(themes["meadow-light"].colors?.surface).toBe("#FFFFFF");
      expect(themes["sprinkles-light"].colors?.background).toBe("#F8FAFC");
      expect(themes["sprinkles-light"].colors?.surface).toBe("#FFFFFF");
    });
  });

  describe("dark themes", () => {
    it("have dark surface colors", () => {
      expect(themes["meadow-dark"].colors?.background).toBe("#0E1512");
      expect(themes["meadow-dark"].colors?.surface).toBe("#16201B");
      expect(themes["sprinkles-dark"].colors?.background).toBe("#0E1512");
      expect(themes["sprinkles-dark"].colors?.surface).toBe("#16201B");
    });
  });

  describe("status colors", () => {
    it("are consistent across all themes", () => {
      const expectedStatus = {
        error: "#EF4444",
        success: "#22C55E",
        warning: "#F59E0B",
        info: "#3B82F6",
      };

      themeNames.forEach((name) => {
        const colors = themes[name].colors!;
        expect(colors.error).toBe(expectedStatus.error);
        expect(colors.success).toBe(expectedStatus.success);
        expect(colors.warning).toBe(expectedStatus.warning);
        expect(colors.info).toBe(expectedStatus.info);
      });
    });
  });

  describe("palette colors", () => {
    it("meadow themes share the same palette", () => {
      expect(themes["meadow-light"].colors?.primary).toBe(
        themes["meadow-dark"].colors?.primary
      );
      expect(themes["meadow-light"].colors?.secondary).toBe(
        themes["meadow-dark"].colors?.secondary
      );
      expect(themes["meadow-light"].colors?.accent).toBe(
        themes["meadow-dark"].colors?.accent
      );
    });

    it("sprinkles themes share the same palette", () => {
      expect(themes["sprinkles-light"].colors?.primary).toBe(
        themes["sprinkles-dark"].colors?.primary
      );
      expect(themes["sprinkles-light"].colors?.secondary).toBe(
        themes["sprinkles-dark"].colors?.secondary
      );
      expect(themes["sprinkles-light"].colors?.accent).toBe(
        themes["sprinkles-dark"].colors?.accent
      );
    });
  });
});
