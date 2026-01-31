import { describe, it, expect, vi, beforeEach } from "vitest";
import { useSearch } from "./useSearch";

const mockPush = vi.fn();

vi.mock("vue-router", () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}));

describe("useSearch", () => {
  beforeEach(() => {
    mockPush.mockClear();
  });

  it("initializes with empty state", () => {
    const { searchQuery, selectedCategoryId, hasSearchQuery } = useSearch();
    expect(searchQuery.value).toBe("");
    expect(selectedCategoryId.value).toBeNull();
    expect(hasSearchQuery.value).toBe(false);
  });

  it("setSearchQuery updates search query", () => {
    const { setSearchQuery, searchQuery, hasSearchQuery } = useSearch();
    setSearchQuery("test query");
    expect(searchQuery.value).toBe("test query");
    expect(hasSearchQuery.value).toBe(true);
  });

  it("setSelectedCategory updates category", () => {
    const { setSelectedCategory, selectedCategoryId } = useSearch();
    setSelectedCategory(5);
    expect(selectedCategoryId.value).toBe(5);
  });

  it("clearSearch resets all values", () => {
    const {
      setSearchQuery,
      setSelectedCategory,
      clearSearch,
      searchQuery,
      selectedCategoryId,
    } = useSearch();

    setSearchQuery("test");
    setSelectedCategory(3);
    clearSearch();

    expect(searchQuery.value).toBe("");
    expect(selectedCategoryId.value).toBeNull();
  });

  it("submitSearch navigates with query params", () => {
    const { setSearchQuery, setSelectedCategory, submitSearch } = useSearch();

    setSearchQuery("laptop");
    setSelectedCategory(2);
    submitSearch();

    expect(mockPush).toHaveBeenCalledWith({
      path: "/products",
      query: { q: "laptop", category: "2" },
    });
  });

  it("submitSearch navigates with only search query", () => {
    const { setSearchQuery, submitSearch } = useSearch();

    setSearchQuery("phone");
    submitSearch();

    expect(mockPush).toHaveBeenCalledWith({
      path: "/products",
      query: { q: "phone" },
    });
  });

  it("submitSearch navigates with only category", () => {
    const { setSelectedCategory, submitSearch } = useSearch();

    setSelectedCategory(4);
    submitSearch();

    expect(mockPush).toHaveBeenCalledWith({
      path: "/products",
      query: { category: "4" },
    });
  });

  it("submitSearch trims whitespace from query", () => {
    const { setSearchQuery, submitSearch } = useSearch();

    setSearchQuery("  spaced query  ");
    submitSearch();

    expect(mockPush).toHaveBeenCalledWith({
      path: "/products",
      query: { q: "spaced query" },
    });
  });

  it("hasSearchQuery is false for whitespace-only query", () => {
    const { setSearchQuery, hasSearchQuery } = useSearch();
    setSearchQuery("   ");
    expect(hasSearchQuery.value).toBe(false);
  });
});
