import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { fireEvent } from "@testing-library/vue";
import ThemeToggler from "./ThemeToggler.vue";
import { useAppStore } from "@/stores/app";

describe("ThemeToggler", () => {
  it("renders toggle button", async () => {
    const { container } = await renderWithPlugins(ThemeToggler, {});
    const button = container.querySelector(".theme-toggle");
    expect(button).toBeDefined();
  });

  it("toggles theme mode when clicked", async () => {
    const { container } = await renderWithPlugins(ThemeToggler, {});
    // Get store AFTER render so Pinia is initialized
    const appStore = useAppStore();
    const button = container.querySelector<HTMLButtonElement>(".theme-toggle");

    expect(appStore.mode).toBe("light");

    await fireEvent.click(button!);
    expect(appStore.mode).toBe("dark");

    await fireEvent.click(button!);
    expect(appStore.mode).toBe("light");
  });

  it("shows correct aria-label based on mode", async () => {
    const { container } = await renderWithPlugins(ThemeToggler, {});
    const button = container.querySelector(".theme-toggle");

    expect(button?.getAttribute("aria-label")).toBe("Switch to dark mode");

    await fireEvent.click(button!);
    expect(button?.getAttribute("aria-label")).toBe("Switch to light mode");
  });

  it("applies toggled class when in dark mode", async () => {
    const { container } = await renderWithPlugins(ThemeToggler, {});
    const button = container.querySelector(".theme-toggle");

    expect(button?.classList.contains("theme-toggle--toggled")).toBe(false);

    await fireEvent.click(button!);
    expect(button?.classList.contains("theme-toggle--toggled")).toBe(true);
  });

  it("shows correct title based on mode", async () => {
    const { container } = await renderWithPlugins(ThemeToggler, {});
    const button = container.querySelector(".theme-toggle");

    expect(button?.getAttribute("title")).toBe("Switch to dark mode");

    await fireEvent.click(button!);
    expect(button?.getAttribute("title")).toBe("Switch to light mode");
  });
});
