import { describe, it, expect } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent, waitFor } from "@testing-library/vue";
import LangSelector from "./LangSelector.vue";

describe("LangSelector", () => {
  it("renders button with current locale", async () => {
    await renderWithPlugins(LangSelector);

    const button = screen.getByRole("button");
    expect(button).toBeDefined();
    expect(button.textContent?.trim()).toBe("en");
  });

  it("has menu trigger attributes", async () => {
    await renderWithPlugins(LangSelector);

    const button = screen.getByRole("button");
    expect(button.getAttribute("aria-haspopup")).toBe("menu");
  });

  it("opens menu on hover and shows available locales", async () => {
    await renderWithPlugins(LangSelector);

    const button = screen.getByRole("button");
    await fireEvent.mouseEnter(button);

    // Vuetify teleports menus to body, so search in document
    await waitFor(() => {
      const listItems = document.querySelectorAll(".v-list-item-title");
      expect(listItems.length).toBeGreaterThan(0);
    });
  });

  it("changes locale when clicking on a different language", async () => {
    await renderWithPlugins(LangSelector);

    const button = screen.getByRole("button");
    expect(button.textContent?.trim()).toBe("en");

    // Open the menu
    await fireEvent.mouseEnter(button);

    // Wait for menu to open and find ES option in document body
    await waitFor(async () => {
      const listItems = document.querySelectorAll(".v-list-item-title");
      const esOption = Array.from(listItems).find(
        (item) => item.textContent?.trim() === "ES"
      );
      expect(esOption).toBeDefined();
      if (esOption) {
        await fireEvent.click(esOption);
      }
    });

    // After clicking, the button should show the new locale
    await waitFor(() => {
      const updatedButton = screen.getByRole("button");
      expect(updatedButton.textContent?.trim()).toBe("es");
    });
  });

  it("displays locale options in uppercase", async () => {
    await renderWithPlugins(LangSelector);

    const button = screen.getByRole("button");
    await fireEvent.mouseEnter(button);

    await waitFor(() => {
      const listItems = document.querySelectorAll(".v-list-item-title");
      const texts = Array.from(listItems).map((item) =>
        item.textContent?.trim()
      );
      expect(texts.length).toBeGreaterThan(0);
      // All should be uppercase
      texts.forEach((text) => {
        expect(text).toBe(text?.toUpperCase());
      });
    });
  });
});
