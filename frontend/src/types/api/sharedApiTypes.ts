export type Paged<T> = {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
};

export type QueryDefaults = {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  sortDescending?: boolean;
  isActive?: boolean;
};

export const DEFAULT_QUERY: Required<Pick<QueryDefaults, "sortDescending">> = {
  sortDescending: true,
};
