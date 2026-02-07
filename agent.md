# Commerce Vue.NET - Agent Guide

## Project Summary

- Full-stack e-commerce app with Vue 3 (frontend) and .NET 8 (backend)
- Local development is Docker-first with PostgreSQL and MinIO

## Core Stack & Ports

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

# Tests
cd frontend && npm run test
cd backend/tests/Commerce.UnitTests && dotnet test
```

## Repo Structure (Key Paths)

```
backend/
  src/
    Commerce.Api/          # Controllers, DI, Swagger
    Commerce.Services/     # Business logic, DTOs, mappers
    Commerce.Repositories/ # EF Core, entities, migrations
    Commerce.Shared/       # Validators, utilities
  tests/
frontend/
  src/
    pages/                 # File-based routing
    components/            # Vuetify components by domain
    composables/           # Business logic (useProducts, etc.)
    services/              # API client layer (*Api.ts)
    types/api/             # TypeScript interfaces
    i18n/                  # Translations
```

## Architecture Patterns

- Backend services return a strongly-typed Result pattern; controllers map to HTTP codes
- Frontend API services return `ApiResult<T>` for consistent error handling
- Composables wrap API calls with reactive state (e.g., `useProducts`)
- Generic `ImageAsset` pipeline handles images for any entity type (Product, Category) via `ImageAssetType` enum + `OwnerId`

## Local Conventions

- TypeScript: do not use `any`
- Target 75%+ coverage for frontend and backend
- Use `@` alias for frontend imports; avoid exact relative paths

## Environment & Access

- Copy `.env.example` to `.env` and edit as needed
- Frontend: http://localhost:5173
- Backend: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- MinIO: http://localhost:9001

## Workflow (Typical Feature)

Backend:

1. Entity + migration
2. Repository
3. DTOs + Service
4. Controller

Frontend:

1. Types
2. API service
3. Composable
4. Components + Page

## Git Workflow

- Do not work directly on `main`
- Create a feature/chore branch
- Run tests before pushing

## References

- `docs/README.md` for full documentation index
- `docs/development/` for setup, workflows, and testing
- `backend/CLAUDE.md` and `frontend/CLAUDE.md` for layer-specific guidance
