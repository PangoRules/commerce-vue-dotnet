/// <reference types="vitest" />
import { defineConfig } from "vitest/config";
import { loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import vuetify from "vite-plugin-vuetify";
import VueI18nPlugin from "@intlify/unplugin-vue-i18n/vite";
import VueRouter from "unplugin-vue-router/vite";
import { fileURLToPath, URL } from "node:url";
import VueDevTools from "vite-plugin-vue-devtools";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  const apiBaseUrl = env.VITE_API_BASE_URL || "http://localhost:8080";

  return {
    plugins: [
      VueRouter({
        routesFolder: "src/pages",
        dts: "src/typed-router.d.ts",
      }),
      VueI18nPlugin({
        include: fileURLToPath(new URL("./src/i18n/locales/**", import.meta.url)),
      }),
      vue(),
      VueDevTools(),
      vuetify({ autoImport: true }),
    ],
    server: {
      proxy: {
        "/api": {
          target: apiBaseUrl,
          changeOrigin: true,
        },
      },
    },
    ssr: {
      noExternal: ["vuetify"],
    },
    test: {
      environment: "jsdom",
      globals: true,
      setupFiles: ["./src/tests/setup.ts"],
      include: ["src/**/*.{test,spec}.ts", "src/**/*.{test,spec}.tsx"],
      css: true,
      restoreMocks: true,
      clearMocks: true,
      mockReset: true,
      deps: {
        optimizer: {
          web: {
            include: ["vuetify"],
          },
        },
      },
      coverage: {
        provider: "v8",
        reporter: ["text", "html", "json-summary"],
        reportsDirectory: "./coverage",
        thresholds: {
          lines: 80,
          functions: 80,
          branches: 80,
          statements: 80,
        },
        include: ["src/**/*.{ts,tsx,vue}"],
        exclude: [
          "src/main.ts",
          "src/plugins/**",
          "src/router/**",
          "src/i18n/**",
          "src/types/**",
          "src/tests/**",
          "**/*.d.ts",
          "**/*.config.*",
          "**/vite.config.*",
          "src/config/**",
          "src/App.vue",
          "src/services/apiClient.ts",
          "src/components/products/ProductCategorySection.vue",
        ],
      },
    },
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
  };
});
