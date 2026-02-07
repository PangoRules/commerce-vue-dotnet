# Backend - Claude Code Guide

## Architecture Overview

Clean Architecture with 4 layers:

```
Commerce.Api (Controllers, DI)
    ↓
Commerce.Services (Business Logic, DTOs)
    ↓
Commerce.Repositories (EF Core, Entities)
    ↓
Commerce.Shared (Validation, Utilities)
```

## Layer Responsibilities

### Commerce.Api
- Thin controllers that delegate to services
- Dependency injection configuration
- Swagger/OpenAPI documentation
- Request/response mapping to HTTP status codes

### Commerce.Services
- Business logic and validation
- Service interfaces and implementations
- DTOs for API contracts (never expose entities)
- Mappers for Entity ↔ DTO conversion

### Commerce.Repositories
- EF Core DbContext (`CommerceDbContext`)
- Entity configurations (Fluent API)
- Repository pattern with interfaces
- Database migrations

### Commerce.Shared
- FluentValidation validators
- Common utilities and extensions
- Shared constants and enums

## Coding Standards

- Use `async/await` for all I/O operations
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- PascalCase for public members (Microsoft conventions)
- Controllers: Minimal logic, proper HTTP status codes
- Services: Business logic, validation, error handling
- Repositories: Data access only, no business logic
- Use `ILogger` for logging

## Feature Development Workflow

1. **Entity**: Create in `Commerce.Repositories/Entities/`
2. **Configuration**: Add EF config in `Commerce.Repositories/Configurations/`
3. **Migration**: `dotnet ef migrations add <Name> --project ../Commerce.Repositories`
4. **Repository**: Interface + implementation in `Commerce.Repositories/`
5. **DTOs**: Create in `Commerce.Services/DTOs/`
6. **Service Interface**: Define in `Commerce.Services/Interfaces/`
7. **Service Implementation**: Implement with business logic
8. **Controller**: Add in `Commerce.Api/Controllers/`
9. **Unit Tests**: Write in `Commerce.UnitTests/`
10. **Integration Tests**: Write in `Commerce.IntegrationTests/`

## Commands

```bash
# Run the API
cd backend/src/Commerce.Api
dotnet run

# Run with watch
dotnet watch run

# Run unit tests
cd backend/tests/Commerce.UnitTests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run integration tests
cd backend/tests/Commerce.IntegrationTests
dotnet test

# EF Core Migrations (from Commerce.Api directory)
dotnet ef migrations add <Name> --project ../Commerce.Repositories
dotnet ef database update --project ../Commerce.Repositories
dotnet ef migrations list --project ../Commerce.Repositories
```

## Current Entities

| Entity | Purpose | File |
|--------|---------|------|
| Product | Core product data | `Entities/Product.cs` |
| ImageAsset | Generic images for any entity (MinIO) | `Entities/ImageAsset.cs` |
| Category | Product categories | `Entities/Category.cs` |
| CategoryLink | Category hierarchy | `Entities/CategoryLink.cs` |

> **Note**: `ImageAsset` uses an `ImageAssetType` enum (`Product`, `Category`) and an `OwnerId` int instead of a direct FK. This allows a single image pipeline to serve multiple entity types.

## Key Files

| Purpose | Path |
|---------|------|
| Entry point | `src/Commerce.Api/Program.cs` |
| Configuration | `src/Commerce.Api/appsettings.json` |
| DbContext | `src/Commerce.Repositories/CommerceDbContext.cs` |
| DI Setup | `src/Commerce.Api/Program.cs` (services registration) |

## Testing Strategy

- **Unit Tests**: Service layer with mocked repositories
- **Integration Tests**: Full API tests with Testcontainers
- **Coverage Target**: 75%+
- Framework: xUnit + Moq + FluentAssertions

## Environment Variables

Set via `docker-compose.yml` or `.env`:

```
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=...;Database=...
Storage__Provider=Minio
Storage__Endpoint=localhost:9000
Storage__AccessKey=...
Storage__SecretKey=...
Storage__Bucket=commerce-assets
```
