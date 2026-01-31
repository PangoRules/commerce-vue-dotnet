import { vi } from "vitest";
import { ref } from "vue";
import type { User } from "@/types/api/authTypes";
import type { ProductResponse } from "@/types/api/productTypes";
import type { CategoryResponse } from "@/types/api/categoryTypes";

// ============================================================================
// Mock Data Factories
// ============================================================================

export function createMockUser(overrides: Partial<User> = {}): User {
  return {
    id: 1,
    email: "john@example.com",
    name: "John Doe",
    avatar: null,
    ...overrides,
  };
}

export function createMockCategory(
  overrides: Partial<CategoryResponse> = {},
): CategoryResponse {
  return {
    id: 1,
    name: "Test Category",
    description: null,
    ...overrides,
  };
}

export function createMockProduct(
  overrides: Partial<ProductResponse> = {},
): ProductResponse {
  return {
    id: 1,
    name: "Test Product",
    description: "Test description",
    price: 99.99,
    stockQuantity: 10,
    categoryId: 1,
    isActive: true,
    category: createMockCategory(),
    images: [],
    primaryImageUrl: null,
    ...overrides,
  };
}

// ============================================================================
// Composable Mock Factories
// ============================================================================

export function createMockUseAuth(overrides: Record<string, unknown> = {}) {
  return {
    isAuthenticated: ref(false),
    user: ref<User | null>(null),
    login: vi.fn(),
    logout: vi.fn(),
    toggleAuth: vi.fn(),
    ...overrides,
  };
}

export function createMockUseSearch(overrides: Record<string, unknown> = {}) {
  return {
    searchQuery: ref(""),
    selectedCategoryId: ref<number | null>(null),
    setSearchQuery: vi.fn(),
    setSelectedCategory: vi.fn(),
    submitSearch: vi.fn(),
    ...overrides,
  };
}

export function createMockUseCategories(
  overrides: Record<string, unknown> = {},
) {
  return {
    loadCategoryList: vi.fn(),
    listCategoryResult: ref(null),
    isCategoryListLoading: ref(false),
    ...overrides,
  };
}

// ============================================================================
// Vuetify Mock Helpers
// ============================================================================

export function createMockUseDisplay(
  overrides: Record<string, unknown> = {},
) {
  return {
    smAndDown: ref(false),
    xs: ref(false),
    sm: ref(false),
    md: ref(true),
    lg: ref(false),
    xl: ref(false),
    mobile: ref(false),
    ...overrides,
  };
}
