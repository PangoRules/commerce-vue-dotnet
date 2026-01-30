export const apiRoutes = {
  health: "/health",

  products: {
    list: "/api/Product",
    byId: (productId: number) => `/api/Product/${productId}`,
    create: "/api/Product",
    update: (productId: number) => `/api/Product/${productId}`,
    toggle: (productId: number) => `/api/Product/toggle/${productId}`,
  },

  productImages: {
    /** GET - list all images for a product */
    list: (productId: number) => `/api/product/${productId}/images`,
    /** GET - proxy stream image content (use in img src) */
    get: (imageId: string) => `/api/productimage/${imageId}`,
    /** GET - get image metadata */
    metadata: (imageId: string) => `/api/productimage/${imageId}/metadata`,
    /** POST - upload image (multipart/form-data) */
    upload: (productId: number) => `/api/product/${productId}/images`,
    /** DELETE - delete an image */
    delete: (imageId: string) => `/api/productimage/${imageId}`,
    /** PUT - set image as primary */
    setPrimary: (imageId: string) => `/api/productimage/${imageId}/primary`,
    /** PUT - reorder images */
    reorder: (productId: number) => `/api/product/${productId}/images/reorder`,
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
