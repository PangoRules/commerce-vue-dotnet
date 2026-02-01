import "vuetify/styles";
import "@testing-library/jest-dom/vitest";
import { afterEach, vi } from "vitest";
import { cleanup } from "@testing-library/vue";

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
