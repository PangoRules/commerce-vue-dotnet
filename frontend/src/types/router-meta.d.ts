import "vue-router";
import type { LayoutKey } from "@/layouts/layouts";

declare module "vue-router" {
  interface RouteMeta {
    layout?: LayoutKey;
  }
}
