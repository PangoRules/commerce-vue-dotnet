import type { CategoryResponse } from "./categoryTypes";
import type { QueryDefaults } from "./sharedApiTypes";

export type ProductImageResponse = {
  id: string;
  productId: number;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  displayOrder: number;
  isPrimary: boolean;
  uploadedAt: string;
  url: string;
};

export type ProductResponse = {
  id: number;
  categoryId: number;
  name: string;
  description?: string | null | undefined;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  category: CategoryResponse | null | undefined;
  images: ProductImageResponse[];
  primaryImageUrl: string | null;
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

export type ReorderImagesRequest = {
  imageIds: string[];
};
