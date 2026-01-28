import {
  render,
  type RenderOptions,
  type RenderResult,
} from "@testing-library/vue";
import { createTestingPinia } from "@pinia/testing";
import {
  createRouter,
  createMemoryHistory,
  type RouteRecordRaw,
  type Router,
} from "vue-router";
import { createI18n } from "vue-i18n";
import { createVuetify } from "vuetify";
import type { Component } from "vue";

import { aliases, mdi } from "vuetify/iconsets/mdi-svg";

export type RenderWithPluginsOptions<C extends Component> = {
  // our custom options
  routes?: RouteRecordRaw[];
  initialRoute?: string;
  stubActions?: boolean;

  // pass-through options for @testing-library/vue
  render?: RenderOptions<C>;
};

export type RenderWithPluginsResult = RenderResult & { router: Router };

export async function renderWithPlugins<C extends Component>(
  component: C,
  opts: RenderWithPluginsOptions<C> = {},
): Promise<RenderWithPluginsResult> {
  const vuetify = createVuetify({
    icons: {
      defaultSet: "mdi",
      aliases,
      sets: { mdi },
    },
  });

  const i18n = createI18n({
    legacy: false,
    locale: "en",
    messages: { en: {} },
  });

  const router = createRouter({
    history: createMemoryHistory(),
    routes: opts.routes?.length
      ? opts.routes
      : [{ path: "/", component: { template: "<div />" } }],
  });

  router.push("/");

  const pinia = createTestingPinia({
    stubActions: opts.stubActions ?? false,
  });

  if (opts.initialRoute && opts.initialRoute.trim().length > 0) {
    router.push(opts.initialRoute);
  }

  const renderOptions = opts.render;

  const result = render(component, {
    ...renderOptions,
    global: {
      ...renderOptions?.global,
      plugins: [
        vuetify,
        i18n,
        router,
        pinia,
        ...(renderOptions?.global?.plugins ?? []),
      ],
    },
  });

  await router.isReady();

  return { ...result, router };
}
