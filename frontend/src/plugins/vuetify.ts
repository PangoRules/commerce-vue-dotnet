import "vuetify/styles";
import { createVuetify } from "vuetify";
import "@mdi/font/css/materialdesignicons.css";
import { DEFAULT_THEME, themes } from "@/theme/themes";

export const vuetify = createVuetify({
  theme: {
    defaultTheme: DEFAULT_THEME,
    themes,
  },
});
