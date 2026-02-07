import type { CategoryResponse } from "@/types/api/categoryTypes";
import type { ImageAssetResponse } from "@/types/api/imageAssetTypes";
import type { QueryDefaults } from "@/types/api/sharedApiTypes";

export type ProductResponse = {
  id: number;
  categoryId: number;
  name: string;
  description?: string | null | undefined;
  price: number;
  salePrice?: number | null | undefined;
  stockQuantity: number;
  isActive: boolean;
  category: CategoryResponse | null | undefined;
  images: ImageAssetResponse[];
  primaryImageUrl: string | null;
};

export type ProductListQuery = QueryDefaults & {
  categoryId?: number;
  isActive?: boolean;
  onSale?: boolean;
};

export type ProductMap = Record<string, ProductResponse>;

export type ProductRequest = {
  categoryId: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
};
