# Development Workflows

## Git Workflow

### Branch Strategy

```
main (protected)
  └── feat/feature-name
  └── fix/bug-description
  └── chore/task-description
```

**Rules:**
- Never commit directly to `main`
- Create feature branches for all work
- Validate code compiles and tests pass before pushing
- User creates PRs manually in GitHub

### Branch Naming

| Prefix | Use Case |
|--------|----------|
| `feat/` | New features |
| `fix/` | Bug fixes |
| `chore/` | Maintenance, dependencies, docs |
| `refactor/` | Code restructuring |

## Feature Development Workflow

### Backend Feature

Example: Adding "Product Reviews"

```bash
# 1. Create branch
git checkout -b feat/product-reviews

# 2. Create Entity
# backend/src/Commerce.Repositories/Entities/ProductReview.cs

# 3. Add EF Configuration
# backend/src/Commerce.Repositories/Configurations/ProductReviewConfiguration.cs

# 4. Create Migration
cd backend/src/Commerce.Api
dotnet ef migrations add AddProductReviews --project ../Commerce.Repositories

# 5. Apply Migration
dotnet ef database update --project ../Commerce.Repositories

# 6. Create Repository
# Interface: Commerce.Repositories/Interfaces/IProductReviewRepository.cs
# Implementation: Commerce.Repositories/ProductReviewRepository.cs

# 7. Create DTOs
# Commerce.Services/DTOs/ProductReviewDto.cs
# Commerce.Services/DTOs/CreateProductReviewDto.cs

# 8. Create Service
# Interface: Commerce.Services/Interfaces/IProductReviewService.cs
# Implementation: Commerce.Services/ProductReviewService.cs

# 9. Create Controller
# Commerce.Api/Controllers/ProductReviewController.cs

# 10. Write Tests
# Commerce.Services.Tests/ProductReviewServiceTests.cs
cd backend/tests/Commerce.Services.Tests
dotnet test

# 11. Commit
git add .
git commit -m "feat: add product reviews API"
```

### Frontend Feature

Example: Adding product reviews UI

```bash
# 1. Create Types
# frontend/src/types/api/productReviewTypes.ts

# 2. Create API Service
# frontend/src/services/productReviewApi.ts

# 3. Write API Tests
# frontend/src/services/productReviewApi.spec.ts

# 4. Create Composable
# frontend/src/composables/useProductReviews.ts

# 5. Write Composable Tests
# frontend/src/composables/useProductReviews.spec.ts

# 6. Create Components
# frontend/src/components/reviews/ReviewCard.vue
# frontend/src/components/reviews/ReviewList.vue
# frontend/src/components/reviews/ReviewForm.vue

# 7. Add to Page
# frontend/src/pages/products/[id].vue

# 8. Add i18n
# frontend/src/i18n/locales/en.json
# frontend/src/i18n/locales/es.json

# 9. Run Tests
cd frontend
npm run test:run
npm run test:coverage

# 10. Commit
git add .
git commit -m "feat: add product reviews UI"
```

## Database Migrations

### Creating Migrations

```bash
cd backend/src/Commerce.Api

# Create migration
dotnet ef migrations add <MigrationName> --project ../Commerce.Repositories

# Example names:
# AddProductReviews
# AddUserTable
# UpdateProductSchema
```

### Applying Migrations

```bash
# Apply all pending migrations
dotnet ef database update --project ../Commerce.Repositories

# Apply to specific migration
dotnet ef database update <MigrationName> --project ../Commerce.Repositories
```

### Reverting Migrations

```bash
# Revert to previous migration
dotnet ef database update <PreviousMigrationName> --project ../Commerce.Repositories

# Remove last migration (if not applied)
dotnet ef migrations remove --project ../Commerce.Repositories
```

### Listing Migrations

```bash
dotnet ef migrations list --project ../Commerce.Repositories
```

## Docker Commands

### Infrastructure Only

```bash
# Start PostgreSQL and MinIO
docker compose --profile infra up -d

# Stop infrastructure
docker compose --profile infra down

# View logs
docker compose --profile infra logs -f
```

### Full Stack

```bash
# Start everything
docker compose --profile infra --profile app up

# Start in background
docker compose --profile infra --profile app up -d

# Rebuild and start
docker compose --profile infra --profile app up --build
```

### Individual Services

```bash
# Restart specific service
docker compose restart backend

# View service logs
docker compose logs -f backend

# Execute command in container
docker compose exec postgres psql -U commerce -d commerce_db
```

### Cleanup

```bash
# Stop all
docker compose down

# Stop and remove volumes
docker compose down -v

# Remove unused images
docker image prune
```

## Testing Workflow

### Backend Tests

```bash
cd backend/tests/Commerce.Services.Tests

# Run all tests
dotnet test

# Run with output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~ProductServiceTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Tests

```bash
cd frontend

# Watch mode (development)
npm run test

# Single run
npm run test:run

# With coverage
npm run test:coverage

# With UI
npm run test:ui
```

## Code Quality Checks

### Before Committing

```bash
# Backend
cd backend
dotnet build  # Must compile
cd tests/Commerce.Services.Tests
dotnet test   # Must pass

# Frontend
cd frontend
npm run build    # Must build (includes type check)
npm run test:run # Must pass
```

### TypeScript Standards

- No `any` types
- Proper typing for all functions
- Interfaces for API contracts

### Coverage Targets

| Area | Target |
|------|--------|
| Backend Services | 75%+ |
| Frontend Composables | 75%+ |
| Frontend Services | 75%+ |
