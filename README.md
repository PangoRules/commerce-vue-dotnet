# Commerce App — Vue 3 + .NET 8 (Dockerized Development Environment) EDIT

A full-stack e-commerce application built with Vue 3 and .NET 8, containerized with Docker.

## Tech Stack

| Layer          | Technology                        |
| -------------- | --------------------------------- |
| Frontend       | Vue 3, TypeScript, Vuetify 3      |
| Backend        | .NET 8, ASP.NET Core, EF Core     |
| Database       | PostgreSQL 16                     |
| Object Storage | MinIO (S3-compatible)             |
| Testing        | Vitest (frontend), xUnit (backend)|

## Quick Start

### Prerequisites

- Docker & Docker Compose v2+
- Node.js 20+ (for local frontend dev)
- .NET 8 SDK (for local backend dev)

### Run with Docker

```bash
# Copy environment file
cp .env.example .env

# Start infrastructure (PostgreSQL + MinIO)
docker compose --profile infra up -d

# Start full stack
docker compose --profile infra --profile app up
```

### Access Points

| Service        | URL                                  |
| -------------- | ------------------------------------ |
| Frontend       | http://localhost:5173                |
| Backend API    | http://localhost:8080                |
| Swagger        | http://localhost:8080/swagger        |
| MinIO Console  | http://localhost:9001                |

## Project Structure

```
commerce-vue-dotnet/
├── frontend/                   # Vue 3 + TypeScript
│   └── src/
│       ├── pages/              # File-based routing
│       ├── components/         # Vuetify components
│       ├── composables/        # Business logic hooks
│       ├── services/           # API client layer
│       └── i18n/               # Translations (en, es)
├── backend/                    # .NET 8 Clean Architecture
│   ├── src/
│   │   ├── Commerce.Api/       # Controllers, DI config
│   │   ├── Commerce.Services/  # Business logic, DTOs
│   │   ├── Commerce.Repositories/  # EF Core, entities
│   │   └── Commerce.Shared/    # Validation, utilities
│   └── tests/
│       ├── Commerce.Services.Tests/      # Unit tests
│       └── Commerce.IntegrationTests/    # API tests
└── docs/                       # Documentation
```

## Local Development

### Frontend

```bash
cd frontend
npm install
npm run dev           # Start dev server
npm run test          # Run tests
npm run test:coverage # Coverage report
```

### Backend

```bash
# Start infrastructure first
docker compose --profile infra up -d

# Run the API
cd backend/src/Commerce.Api
dotnet run

# Run tests
cd backend
dotnet test Commerce.sln
```

## Features

### Implemented

- Product management (CRUD)
- Product images with MinIO storage
- Category management with hierarchy
- Product listing with grid/list views
- Theme support (light/dark)
- i18n (English, Spanish)

### Planned

- User authentication
- Shopping cart
- Order management
- Payment integration

## Database

PostgreSQL runs in Docker with auto-migrations on startup.

```bash
# Reset database (deletes all data)
docker compose down -v
docker compose --profile infra up -d
```

### Connection Details

| Property | Value            |
| -------- | ---------------- |
| Host     | localhost        |
| Port     | 5432             |
| Database | commerce_db      |
| Username | commerce         |
| Password | commerce_password|

## Documentation

Detailed docs are in the `docs/` folder:

- [Architecture Overview](docs/architecture/overview.md)
- [Getting Started](docs/development/getting-started.md)
- [API Conventions](docs/api/conventions.md)
- [Testing Guide](docs/development/testing.md)

## License

MIT
