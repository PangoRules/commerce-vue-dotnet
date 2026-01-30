export type I18nSchema = {
  app: typeof import("@/i18n/locales/en/app.json");
  common: typeof import("@/i18n/locales/en/common.json");
  home: typeof import("@/i18n/locales/en/home.json");
  errors: typeof import("@/i18n/locales/en/errors.json");
  products: typeof import("@/i18n/locales/en/products.json");
};

export type Locales = "en" | "es";
