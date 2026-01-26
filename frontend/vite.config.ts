import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import vuetify from "vite-plugin-vuetify";
import VueI18nPlugin from "@intlify/unplugin-vue-i18n/vite";
import VueRouter from "unplugin-vue-router/vite";
import { fileURLToPath, URL } from "node:url";

export default defineConfig({
  plugins: [
    VueRouter({
      routesFolder: "src/pages",
      dts: "src/typed-router.d.ts",
    }),
    VueI18nPlugin({
      include: fileURLToPath(new URL("./src/i18n/locales/**", import.meta.url)),
    }),
    vue(),
    vuetify({ autoImport: true }),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
