import { useThemeStore } from "@/stores/theme";
import { useTheme } from "vuetify";

export function syncThemeToVuetify() {
  const store = useThemeStore();
  const theme = useTheme();
  theme.global.name.value = store.themeName;
}
