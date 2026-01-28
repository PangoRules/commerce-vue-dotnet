export const apiRoutes = {
  health: "/health",

  products: {
    list: "/api/Product",
    byId: (productId: number) => `/api/Product/${productId}`,
    create: "/api/Product",
    update: (productId: number) => `/api/Product/${productId}`,
    toggle: (productId: number) => `/api/Product/toggle/${productId}`,
  },

  categories: {
    list: "/api/Category",
    byId: (categoryId: number) => `/api/Category/${categoryId}`,
    create: "/api/Category",
    update: (categoryId: number) => `/api/Category/${categoryId}`,
    roots: "/api/Category/roots",
    children: (parentId: number) => `/api/Category/${parentId}/children`,
    toggle: (categoryId: number) => `/api/Category/toggle/${categoryId}`,
    attachChild: (parentId: number, childId: number) =>
      `/api/Category/${parentId}/children/${childId}`,
    detachChild: (parentId: number, childId: number) =>
      `/api/Category/${parentId}/children/${childId}`,
  },
} as const;
