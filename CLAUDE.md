# Commerce Vue.NET - Claude Code Guide

## Quick Reference

| Component      | Technology            | Port                       |
| -------------- | --------------------- | -------------------------- |
| Backend API    | .NET 8 (ASP.NET Core) | 8080                       |
| Frontend       | Vue 3 + Vuetify 3     | 5173                       |
| Database       | PostgreSQL 16         | 5432                       |
| Object Storage | MinIO (S3-compatible) | 9000 (API), 9001 (Console) |

## Essential Commands

```bash
# Infrastructure (PostgreSQL + MinIO)
docker compose --profile infra up -d

# Full stack
docker compose --profile infra --profile app up

# Frontend development
cd frontend && npm run dev

# Backend development
cd backend/src/Commerce.Api && dotnet run

# Run tests
cd frontend && npm run test              # Frontend unit tests
cd backend/tests/Commerce.Services.Tests && dotnet test  # Backend unit tests

# Check coverage
cd frontend && npm run test:coverage     # Target: 75%+
```

## Project Structure

```
commerce-vue-dotnet/
├── backend/                    # .NET 8 Clean Architecture
│   ├── src/
│   │   ├── Commerce.Api/       # Controllers, DI, Swagger (port 8080)
│   │   ├── Commerce.Services/  # Business logic, DTOs, mappers
│   │   ├── Commerce.Repositories/  # EF Core, entities, migrations
│   │   └── Commerce.Shared/    # FluentValidation, utilities
│   └── tests/
├── frontend/                   # Vue 3 + TypeScript
│   └── src/
│       ├── pages/              # File-based routing (unplugin-vue-router)
│       ├── components/         # Vuetify components by domain
│       ├── composables/        # Business logic (useProducts, etc.)
│       ├── services/           # API client layer (*Api.ts)
│       ├── types/api/          # TypeScript interfaces
│       └── i18n/               # Translations (en.json, es.json)
└── docs/                       # Detailed documentation
```

## Key Patterns

### Backend: Result Pattern

Services return strongly-typed results. Controllers map to HTTP status codes.
See: `backend/src/Commerce.Services/Interfaces/`

### Frontend: ApiResult Pattern

```typescript
type ApiResult<T> =
  | { success: true; data: T }
  | { success: false; error: string };
```

All API services return `ApiResult<T>` for consistent error handling.
See: `frontend/src/types/api/`

### Frontend: Composables Pattern

```typescript
// Composables wrap API calls with reactive state
const { products, loading, error, fetchProducts } = useProducts();
```

See: `frontend/src/composables/`

## Code Quality Standards

- **TypeScript**: No `any` types - always use proper typing
- **Testability**: Keep code in small, testable units
- **Coverage targets**: 75%+ for both UI (Vitest) and Backend (xUnit)
- **Scalability**: Follow established patterns for easy extension
  > DO NOT use exact paths. @ has been configured to act as a relative path.

## Git Workflow

- **Never work directly on main** - create feature/chore branches
- Validate code compiles and tests pass before pushing
- Keep main intact until changes are verified
- User creates PRs manually in GitHub

## Current Status

### Implemented ✓

- Product CRUD with images (MinIO storage)
- Category management with hierarchical links
- Product listing page with grid/list views
- API client layer with ApiResult pattern
- Composables for products, categories, product images
- Docker Compose for local development
- i18n support (English, Spanish)

### TODO

- User authentication and authorization
- Shopping cart functionality
- Order management
- Payment integration
- Production deployment configuration

## Documentation Index

| Area            | File                                                                       | Purpose                  |
| --------------- | -------------------------------------------------------------------------- | ------------------------ |
| Hub             | [docs/README.md](docs/README.md)                                           | Documentation navigation |
| Architecture    | [docs/architecture/overview.md](docs/architecture/overview.md)             | System design, data flow |
| Decisions       | [docs/architecture/decisions/](docs/architecture/decisions/)               | ADR log                  |
| Getting Started | [docs/development/getting-started.md](docs/development/getting-started.md) | Setup guide              |
| Workflows       | [docs/development/workflows.md](docs/development/workflows.md)             | Feature development      |
| Testing         | [docs/development/testing.md](docs/development/testing.md)                 | Test strategy            |
| API             | [docs/api/conventions.md](docs/api/conventions.md)                         | Endpoint patterns        |
| Database        | [docs/data/database.md](docs/data/database.md)                             | Schema, migrations       |
| Security        | [docs/security/threat-model-lite.md](docs/security/threat-model-lite.md)   | Security considerations  |

## Subfolder Guides

- [backend/CLAUDE.md](backend/CLAUDE.md) - Backend-specific patterns and commands
- [frontend/CLAUDE.md](frontend/CLAUDE.md) - Frontend-specific patterns and commands

## Key Files Reference

| Purpose           | File                                        |
| ----------------- | ------------------------------------------- |
| Project context   | `.claude-code.json`                         |
| Docker services   | `docker-compose.yml`                        |
| Backend entry     | `backend/src/Commerce.Api/Program.cs`       |
| Backend config    | `backend/src/Commerce.Api/appsettings.json` |
| Frontend scripts  | `frontend/package.json`                     |
| Build/test config | `frontend/vite.config.ts`                   |
| Environment vars  | `.env` (not committed)                      |
