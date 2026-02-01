import { describe, it, expect } from "vitest";
import { layoutKeys, layoutMap, DEFAULT_LAYOUT } from "./layoutTypes";
import AppLayout from "./AppLayout.vue";

describe("layoutTypes", () => {
  describe("layoutKeys", () => {
    it("contains 'app' as a valid layout key", () => {
      expect(layoutKeys).toContain("app");
    });

    it("has exactly one layout key", () => {
      expect(layoutKeys.length).toBe(1);
    });
  });

  describe("layoutMap", () => {
    it("maps 'app' key to AppLayout component", () => {
      expect(layoutMap.app).toBe(AppLayout);
    });

    it("has entries for all layoutKeys", () => {
      for (const key of layoutKeys) {
        expect(layoutMap[key]).toBeDefined();
      }
    });
  });

  describe("DEFAULT_LAYOUT", () => {
    it("is set to 'app'", () => {
      expect(DEFAULT_LAYOUT).toBe("app");
    });

    it("is a valid key in layoutMap", () => {
      expect(layoutMap[DEFAULT_LAYOUT]).toBeDefined();
    });
  });
});
