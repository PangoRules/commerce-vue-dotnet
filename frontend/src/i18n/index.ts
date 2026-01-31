import { createI18n } from "vue-i18n";
import type { I18nSchema, Locales } from "@/i18n/i18types";

// EN
import enApp from "@/i18n/locales/en/app.json";
import enCommon from "@/i18n/locales/en/common.json";
import enHome from "@/i18n/locales/en/home.json";
import enErrors from "@/i18n/locales/en/errors.json";
import enProducts from "@/i18n/locales/en/products.json";
import enNavbar from "@/i18n/locales/en/navbar.json";

// ES
import esApp from "@/i18n/locales/es/app.json";
import esCommon from "@/i18n/locales/es/common.json";
import esHome from "@/i18n/locales/es/home.json";
import esErrors from "@/i18n/locales/es/errors.json";
import esProducts from "@/i18n/locales/es/products.json";
import esNavbar from "@/i18n/locales/es/navbar.json";

const messages = {
  en: {
    app: enApp,
    common: enCommon,
    home: enHome,
    errors: enErrors,
    products: enProducts,
    navbar: enNavbar,
  },
  es: {
    app: esApp,
    common: esCommon,
    home: esHome,
    errors: esErrors,
    products: esProducts,
    navbar: esNavbar,
  },
} satisfies Record<Locales, I18nSchema>;

export const i18n = createI18n<[I18nSchema], Locales>({
  legacy: false,
  globalInjection: true,
  locale: "es",
  fallbackLocale: "en",
  messages,
});
