import "vuetify/styles";
import "@testing-library/jest-dom/vitest";
import { afterEach } from "vitest";
import { cleanup } from "@testing-library/vue";

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
