import AppLayout from "@/layouts/AppLayout.vue";

export const layoutKeys = ["app"] as const;
export type LayoutKey = (typeof layoutKeys)[number];

export const layoutMap: Record<LayoutKey, any> = {
  app: AppLayout,
};

export const DEFAULT_LAYOUT: LayoutKey = "app";
