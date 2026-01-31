export type I18nSchema = {
  app: typeof import("@/i18n/locales/en/app.json");
  categories: typeof import("@/i18n/locales/en/categories.json");
  common: typeof import("@/i18n/locales/en/common.json");
  errors: typeof import("@/i18n/locales/en/errors.json");
  home: typeof import("@/i18n/locales/en/home.json");
  navbar: typeof import("@/i18n/locales/en/navbar.json");
  products: typeof import("@/i18n/locales/en/products.json");
};

export type Locales = "en" | "es";
