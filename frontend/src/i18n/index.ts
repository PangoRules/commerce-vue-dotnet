import { createI18n } from "vue-i18n";
import en from "@/i18n/locales/en.json";
import es from "@/i18n/locales/es.json";
import type { I18nSchema, Locales } from "@/i18n/i18types";

export const i18n = createI18n<[I18nSchema], Locales>({
  legacy: false,
  globalInjection: true,
  locale: "en",
  fallbackLocale: "en",
  messages: {
    en,
    es,
  },
});
