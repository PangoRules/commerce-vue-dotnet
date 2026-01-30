# Frontend - Claude Code Guide

## Architecture Overview

Layered architecture with separation of concerns:

```
Pages (File-based routes)
    ↓
Components (Vuetify UI)
    ↓
Composables (Business logic, state)
    ↓
Services (API client layer)
    ↓
Types (TypeScript interfaces)
```

## Directory Structure

```
src/
├── pages/              # File-based routing (unplugin-vue-router)
├── components/         # Reusable UI components by domain
│   └── shared/         # Shared components (SnackbarHost, etc.)
├── composables/        # Business logic (useProducts, useCategories)
├── services/           # API client layer (productsApi, apiClient)
├── lib/                # Core utilities (http, notify)
├── stores/             # Pinia stores (minimal, prefer composables)
├── types/api/          # TypeScript types by domain
├── config/             # Configuration (apiRoutes, env)
├── layouts/            # Layout components
├── i18n/               # Translations (en.json, es.json)
└── plugins/            # Vue plugins (pinia, vuetify)
```

## Key Patterns

### ApiResult Pattern
All API services return a consistent result type:

```typescript
type ApiResult<T> =
  | { success: true; data: T }
  | { success: false; error: string }
```

See: `src/types/api/`

### Composables Pattern
Composables wrap API calls with reactive state:

```typescript
export function useProducts() {
  const products = ref<Product[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchProducts() {
    loading.value = true
    error.value = null
    const result = await productsApi.getProducts()
    if (result.success) {
      products.value = result.data
    } else {
      error.value = result.error
    }
    loading.value = false
  }

  return { products, loading, error, fetchProducts }
}
```

### Component Standards
- Use `<script setup lang="ts">`
- Define props: `defineProps<{ title: string }>()`
- Define emits: `defineEmits<{ submit: [data: FormData] }>()`
- Use Vuetify components (v-btn, v-card, v-data-table)
- Keep components under 200 lines

## Feature Development Workflow

1. **Types**: Define in `src/types/api/{domain}Types.ts`
2. **API Service**: Create `src/services/{domain}Api.ts`
3. **API Tests**: Write `{domain}Api.spec.ts`
4. **Composable**: Build `src/composables/use{Domain}.ts`
5. **Composable Tests**: Write `use{Domain}.spec.ts`
6. **Components**: Create in `src/components/{domain}/`
7. **Component Tests**: Write tests for interactions
8. **Page**: Create in `src/pages/` (auto-routes)
9. **i18n**: Add translations to `en.json`, `es.json`
10. **Coverage**: Run `npm run test:coverage`

## Commands

```bash
# Development server
npm run dev

# Run tests (watch mode)
npm run test

# Run tests once
npm run test:run

# Run with coverage
npm run test:coverage

# Type check + build
npm run build

# Preview production build
npm run preview
```

## Coding Standards

- **TypeScript**: No `any` types - always use proper typing
- **Components**: `<script setup lang="ts">` syntax
- **Props**: Use `defineProps<T>()` with interface
- **Emits**: Use `defineEmits<T>()` with type
- **State**: Prefer composables over Pinia stores
- **i18n**: All user-facing strings in locale files

## File Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Components | PascalCase | `ProductCard.vue` |
| Composables | camelCase, `use` prefix | `useProducts.ts` |
| Services | camelCase, `Api` suffix | `productsApi.ts` |
| Types | camelCase, `Types` suffix | `productTypes.ts` |
| Tests | Same as source + `.spec.ts` | `useProducts.spec.ts` |

## Current Composables

| Composable | Purpose | File |
|------------|---------|------|
| useProducts | Product CRUD | `composables/useProducts.ts` |
| useCategories | Category management | `composables/useCategories.ts` |
| useProductImages | Image management | `composables/useProductImages.ts` |

## Key Files

| Purpose | Path |
|---------|------|
| Entry point | `src/main.ts` |
| App component | `src/App.vue` |
| HTTP client | `src/lib/http.ts` |
| API routes | `src/config/apiRoutes.ts` |
| Vite config | `vite.config.ts` |
| TS config | `tsconfig.json` |

## Testing Strategy

- **Framework**: Vitest + @testing-library/vue
- **Coverage Target**: 75%+
- **Mock API**: `vi.mock('../services/productsApi')`
- **Test Helpers**: `src/tests/helpers/`

## Environment Variables

```
VITE_API_BASE_URL=http://localhost:8080
```

Set in `.env` or `docker-compose.yml`.
