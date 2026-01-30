# Configurable Theme Palettes (Light/Dark) - Current Implementation

This document describes how theme palettes are implemented in this codebase
(today) and where the related pieces live. It is descriptive, not a step-by-step
recipe.

## Files and responsibilities

- `frontend/src/theme/palettes.ts`
  - Defines the semantic palette list and color tokens.
  - Current palettes: `meadow`, `sprinkles`.
- `frontend/src/theme/themes.ts`
  - Builds Vuetify theme objects for light/dark variants.
  - Adds shared surface/background and status colors.
  - Exports the `themes` map and `ThemeName` type.
- `frontend/src/plugins/vuetify.ts`
  - Registers the `themes` map with Vuetify.
  - Sets the default theme to `meadow-light`.
- `frontend/src/stores/theme.ts`
  - Persists user-selected theme name in `localStorage`.
  - Storage key: `app.theme`.
- `frontend/src/lib/theme.ts`
  - Helper to sync the store selection into Vuetify.

## Palette definitions

`frontend/src/theme/palettes.ts` defines a small set of semantic tokens per
palette:

- `primary`
- `secondary`
- `accent`

Palettes are typed with `PaletteName` and referenced by the theme builder.

## Theme construction

`frontend/src/theme/themes.ts` defines:

- `Mode = "light" | "dark"`
- A `surfaces` map for background/surface per mode
- `makeTheme(name, mode)` to build a Vuetify `ThemeDefinition`

Each theme includes:

- `primary`, `secondary`, `accent` from the palette
- `background`, `surface` from the `surfaces` map
- `error`, `success`, `warning`, `info` tokens as fixed colors

Theme names follow the format `"<palette>-<mode>"`, for example:

- `meadow-light`
- `meadow-dark`
- `sprinkles-light`
- `sprinkles-dark`

The exported `ThemeName` type is a union of those keys.

## Vuetify integration

`frontend/src/plugins/vuetify.ts` registers the `themes` map and sets
`defaultTheme` to `meadow-light`.

## Persistence

`frontend/src/stores/theme.ts` stores the selected theme name in `localStorage`
under `app.theme`. The store initializes from `localStorage` and defaults to
`meadow-light` when no value is present.

## Sync helper

`frontend/src/lib/theme.ts` provides `syncThemeToVuetify()`, which applies the
stored theme to `theme.global.name.value`. This helper currently exists but is
not invoked during app startup.

## Current behavior summary

- Vuetify loads with `meadow-light` by default.
- The theme store can persist a selected theme.
- The persisted theme does not yet get applied automatically on startup because
  `syncThemeToVuetify()` is not called from `frontend/src/main.ts` (or elsewhere).

## Notes for future updates

If you add a new palette:

- Add a palette object in `frontend/src/theme/palettes.ts`.
- Add light/dark theme entries in `frontend/src/theme/themes.ts`.
- Optionally update any UI that allows selection.

If you want the saved theme to apply on load:

- Call `syncThemeToVuetify()` after `app.use(vuetify)` and `app.use(pinia)`.
- Or wire a `watch` in `frontend/src/main.ts` to keep Vuetify in sync with the
  store.
