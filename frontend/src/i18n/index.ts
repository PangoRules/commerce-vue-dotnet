import { createI18n } from "vue-i18n";
import type { I18nSchema, Locales } from "@/i18n/i18types";

// Dynamically import all locale files
// Pattern: ./locales/{locale}/{namespace}.json
const localeModules = import.meta.glob<Record<string, unknown>>(
  "./locales/**/*.json",
  { eager: true },
);

function buildMessages(): Record<Locales, I18nSchema> {
  const messages: Record<string, Record<string, unknown>> = {};

  for (const [path, module] of Object.entries(localeModules)) {
    // Extract locale and namespace from path: ./locales/en/common.json -> ["en", "common"]
    const match = path.match(/\.\/locales\/(\w+)\/(\w+)\.json$/);
    if (!match) continue;

    const locale = match[1];
    const namespace = match[2];

    if (!locale || !namespace) continue;

    if (!messages[locale]) {
      messages[locale] = {};
    }

    messages[locale][namespace] = module.default ?? module;
  }

  return messages as Record<Locales, I18nSchema>;
}

export const messages = buildMessages();

export const i18n = createI18n<[I18nSchema], Locales>({
  legacy: false,
  globalInjection: true,
  locale: "es",
  fallbackLocale: "en",
  messages,
});
