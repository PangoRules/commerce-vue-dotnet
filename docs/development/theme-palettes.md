# Configurable Theme Palettes (Light/Dark) - Current Implementation

This document describes how theme palettes are implemented in this codebase
and where the related pieces live.

## Files and responsibilities

- `frontend/src/theme/palettes.ts`
  - Defines the semantic palette list and color tokens.
  - Current palettes: `meadow`, `sprinkles`.
  - Exports `Palette` interface and `PaletteName` type.
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
- `frontend/src/App.vue`
  - Watches the theme store and syncs changes to Vuetify.

## Palette definitions

`frontend/src/theme/palettes.ts` defines a small set of semantic tokens per
palette:

- `primary`
- `secondary`
- `accent`

Palettes are typed with `PaletteName` and the `Palette` interface ensures
type safety for all palette definitions.

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

## Theme sync

`frontend/src/App.vue` uses a Vue watcher with `immediate: true` to sync the
theme store value to Vuetify's `theme.global.name.value`. This ensures:

1. The saved theme is applied on initial load
2. Any subsequent theme changes are reflected immediately

## Current behavior summary

- Vuetify loads with `meadow-light` by default.
- The theme store persists the selected theme to localStorage.
- On app startup, the watcher in App.vue applies the stored theme.
- Theme changes via the store are automatically synced to Vuetify.

## Notes for future updates

If you add a new palette:

1. Add a palette object in `frontend/src/theme/palettes.ts`.
2. Add the palette name to the `PaletteName` type union.
3. Add light/dark theme entries in `frontend/src/theme/themes.ts`.
4. Optionally update any UI that allows theme selection.
