import type { CategoryResponse } from "./categoryTypes";
import type { QueryDefaults } from "./sharedApiTypes";

export type ProductResponse = {
  id: number;
  categoryId: number;
  name: string;
  description?: string | null | undefined;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  category: CategoryResponse | null | undefined;
};

export type ProductListQuery = QueryDefaults & {
  categoryId?: number;
};

export type ProductMap = Record<string, ProductResponse>;

export type ProductRequest = {
  categoryId: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
};
