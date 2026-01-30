import { createApp, watch } from "vue";
import App from "@/App.vue";
import { pinia } from "@/plugins/pinia";
import { router } from "@/router/index";
import { i18n } from "@/i18n/index";
import { vuetify } from "@/plugins/vuetify";
import { initNotify } from "@/lib/notify";
import { syncThemeToVuetify } from "@/lib/theme";
const app = createApp(App);

app.use(pinia);
initNotify(pinia);
app.use(router);
app.use(i18n);
app.use(vuetify);
syncThemeToVuetify();
app.mount("#app");

const setTitle = () => {
  document.title = i18n.global.t("app.title");
};

setTitle();

watch(
  () => i18n.global.locale,
  () => setTitle(),
  { immediate: true },
);
