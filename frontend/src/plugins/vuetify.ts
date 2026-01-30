import "vuetify/styles";
import { createVuetify } from "vuetify";
import "@mdi/font/css/materialdesignicons.css";
import { themes } from "@/theme/themes";

export const vuetify = createVuetify({
  theme: {
    defaultTheme: "meadow-light",
    themes,
  },
});
