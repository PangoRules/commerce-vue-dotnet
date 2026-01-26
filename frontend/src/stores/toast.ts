import { defineStore } from "pinia";

export type ToastLevel = "success" | "info" | "warning" | "error";

export const useToastStore = defineStore("toast", {
  state: () => ({
    show: false,
    text: "",
    level: "info" as ToastLevel,
    timeout: 4000,
  }),
  actions: {
    open(text: string, level: ToastLevel = "info", timeout = 4000) {
      this.text = text;
      this.level = level;
      this.timeout = timeout;
      this.show = true;
    },
    success(text: string) {
      this.open(text, "success");
    },
    info(text: string) {
      this.open(text, "info");
    },
    warning(text: string) {
      this.open(text, "warning");
    },
    error(text: string) {
      this.open(text, "error", 6000);
    },
  },
});
