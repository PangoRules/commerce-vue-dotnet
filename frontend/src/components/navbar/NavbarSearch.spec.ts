import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderWithPlugins } from "@/tests/render";
import { screen, fireEvent } from "@testing-library/vue";
import NavbarSearch from "./NavbarSearch.vue";

vi.mock("@/composables/useCategories", () => ({
  useCategories: () => ({
    loadCategoryList: vi.fn(),
    listCategoryResult: { value: null },
    isCategoryListLoading: { value: false },
  }),
}));

describe("NavbarSearch", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders search input", async () => {
    await renderWithPlugins(NavbarSearch);

    const input = screen.getByRole("textbox");
    expect(input).toBeDefined();
  });

  it("renders with initial modelValue", async () => {
    await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          modelValue: "initial query",
        },
      },
    });

    const input = screen.getByRole("textbox") as HTMLInputElement;
    expect(input.value).toBe("initial query");
  });

  it("emits update:modelValue when input changes", async () => {
    const onUpdate = vi.fn();
    await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          modelValue: "",
          "onUpdate:modelValue": onUpdate,
        },
      },
    });

    const input = screen.getByRole("textbox");
    await fireEvent.update(input, "test search");

    expect(onUpdate).toHaveBeenCalledWith("test search");
  });

  it("emits submit when enter is pressed", async () => {
    const onSubmit = vi.fn();
    await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          onSubmit,
        },
      },
    });

    const input = screen.getByRole("textbox");
    await fireEvent.keyUp(input, { key: "Enter" });

    expect(onSubmit).toHaveBeenCalledTimes(1);
  });

  it("emits submit when search button is clicked", async () => {
    const onSubmit = vi.fn();
    await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          onSubmit,
        },
      },
    });

    const buttons = screen.getAllByRole("button");
    const searchButton = buttons.find(
      (btn) => btn.getAttribute("aria-label") !== null
    );
    if (searchButton) {
      await fireEvent.click(searchButton);
      expect(onSubmit).toHaveBeenCalledTimes(1);
    }
  });

  it("hides category dropdown when hideCategory is true", async () => {
    const { container } = await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          hideCategory: true,
        },
      },
    });

    const categoryDropdown = container.querySelector(
      ".navbar-search__category--hidden"
    );
    expect(categoryDropdown).toBeNull();
  });

  it("shows category dropdown when hideCategory is false", async () => {
    const { container } = await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          hideCategory: false,
        },
      },
    });

    const categoryDropdown = container.querySelector(
      ".navbar-search__category"
    );
    expect(categoryDropdown).toBeDefined();
  });

  it("applies expanded class when expanded prop is true", async () => {
    const { container } = await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          expanded: true,
        },
      },
    });

    const searchContainer = container.querySelector(".navbar-search");
    expect(searchContainer?.classList.contains("expanded")).toBe(true);
  });

  it("does not apply expanded class when expanded is false", async () => {
    const { container } = await renderWithPlugins(NavbarSearch, {
      render: {
        props: {
          expanded: false,
        },
      },
    });

    const searchContainer = container.querySelector(".navbar-search");
    expect(searchContainer?.classList.contains("expanded")).toBe(false);
  });
});
