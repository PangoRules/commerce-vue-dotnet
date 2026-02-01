import { defineStore } from "pinia";
import { getThemeName, type Mode, type ThemeName } from "@/theme/themes";
import { computed, ref } from "vue";

export const useThemeStore = defineStore(
  "theme",
  () => {
    const mode = ref<Mode>("light");

    const themeName = computed<ThemeName>(() => getThemeName(mode.value));

    function setMode(newMode: Mode) {
      mode.value = newMode;
    }
    function toggleMode() {
      setMode(mode.value === "light" ? "dark" : "light");
    }

    return { themeName, mode, setMode, toggleMode };
  },
  {
    persist: {
      key: "app.theme",
    },
  },
);
