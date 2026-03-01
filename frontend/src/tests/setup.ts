import "vuetify/styles";
import "@testing-library/jest-dom/vitest";
import { afterEach, vi } from "vitest";
import { cleanup } from "@testing-library/vue";

// Suppress Vue Router 5 deprecation warning triggered by Vuetify's router
// composable which still uses the old next() callback style. This is a Vuetify
// bug — remove this once Vuetify ships a fix.
const originalWarn = console.warn;
console.warn = (...args: Parameters<typeof console.warn>) => {
  if (typeof args[0] === "string" && args[0].includes("next()` callback in navigation guards is deprecated")) {
    return;
  }
  originalWarn(...args);
};

// Mock ResizeObserver for Vuetify components
class ResizeObserverMock {
  observe = vi.fn();
  unobserve = vi.fn();
  disconnect = vi.fn();
}

globalThis.ResizeObserver =
  ResizeObserverMock as unknown as typeof ResizeObserver;

if (!("visualViewport" in window)) {
  Object.defineProperty(window, "visualViewport", {
    value: {
      width: 1024,
      height: 768,
      scale: 1,
      offsetLeft: 0,
      offsetTop: 0,
      addEventListener: () => {},
      removeEventListener: () => {},
    },
    configurable: true,
  });
}

afterEach(() => cleanup());
