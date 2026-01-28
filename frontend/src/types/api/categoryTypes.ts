import type { QueryDefaults } from "@/types/api/sharedApiTypes";

export type CategoryResponse = {
  id: number;
  name: string;
  description: string | null | undefined;
};

export type CategoryNameIdPair = {
  id: number;
  name: string;
};

export type CategoryAdminDetailsResponse = {
  id: number;
  name: string;
  description: string | null | undefined;
  isActive: boolean;
  parents: CategoryNameIdPair[];
  children: CategoryNameIdPair[];
};

export type CategoryListQuery = QueryDefaults;

export type CategoryRequest = {
  name: string;
  description: string;
  parentCategoryIds: number[];
};
