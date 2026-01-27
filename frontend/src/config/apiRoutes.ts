// src/config/apiRoutes.ts
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
    // add more as you build them
  },
} as const;
