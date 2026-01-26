export type Paged<T> = {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
};
