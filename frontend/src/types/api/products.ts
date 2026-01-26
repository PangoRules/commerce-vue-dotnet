import type { CategoryResponse } from "./categories";

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

export type ProductListQuery = {
  search?: string;
  categoryId?: number;
  page?: number;
  pageSize?: number;
};

export type ProductMap = Record<string, ProductResponse>;
