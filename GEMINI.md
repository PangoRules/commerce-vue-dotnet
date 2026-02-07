# Gemini Code Guide - Commerce Vue.NET

This document provides a summary of the project's architecture, technology stack, and development workflows, generated from the existing `README.md` and `CLAUDE.md` files.

## Project Overview

**Commerce Vue.NET** is a full-stack e-commerce application designed for a Docker-based local development environment. It features a Vue.js frontend and a .NET backend.

| Component      | Technology            | Port                       |
| -------------- | --------------------- | -------------------------- |
| Backend API    | .NET 8 (ASP.NET Core) | 8080                       |
| Frontend       | Vue 3 + Vuetify 3     | 5173                       |
| Database       | PostgreSQL 16         | 5432                       |
| Object Storage | MinIO (S3-compatible) | 9000 (API), 9001 (Console) |

## Getting Started

### Prerequisites
- Docker
- Docker Compose v2 or newer

### Setup Steps
1.  **Set up environment variables:**
    ```bash
    cp .env.example .env
    ```
    Update the `.env` file with your local configuration if necessary.

2.  **Run the full stack:**
    ```bash
    # Recommended for full app experience
    docker compose --profile infra --profile app up --build
    ```
    Alternatively, to start only the base infrastructure (Postgres & MinIO):
    ```bash
    docker compose --profile infra up -d
    ```

### Access Points
-   **Frontend:** [http://localhost:5173](http://localhost:5173)
-   **Backend API:** [http://localhost:8080](http://localhost:8080)
-   **Swagger UI:** [http://localhost:8080/swagger](http://localhost:8080/swagger)
-   **MinIO Console:** [http://localhost:9001](http://localhost:9001)

---

## Technology Stack

### Backend (.NET 8)
-   **Architecture**: Follows a Clean Architecture pattern with four distinct layers:
    1.  `Commerce.Api`: Controllers, Dependency Injection, and API entry points.
    2.  `Commerce.Services`: Business logic, DTOs, and mappers.
    3.  `Commerce.Repositories`: Data access using Entity Framework Core.
    4.  `Commerce.Shared`: Common utilities, validators, and enums.
-   **Key Patterns**: Uses a `Result` pattern in services for robust error handling. Generic `ImageAsset` pipeline supports images for any entity type (Product, Category) via `ImageAssetType` enum and `OwnerId`.
-   **Testing**: Employs xUnit, Moq, and FluentAssertions for unit tests, with Testcontainers for integration tests.

### Frontend (Vue 3)
-   **Framework**: Vue 3 with Vuetify 3 for UI components.
-   **Language**: TypeScript (`<script setup lang="ts">`).
-   **Architecture**:
    1.  `Pages`: File-based routing via `unplugin-vue-router`.
    2.  `Components`: Reusable UI elements, organized by feature.
    3.  `Composables`: Manages state and business logic (e.g., `useProducts`).
    4.  `Services`: Handles API communication.
-   **Key Patterns**:
    -   `ApiResult<T>` pattern for consistent API error handling.
    -   Composables for reactive state management.
    -   Home page features: FeaturedCategories (crossfade image slideshow), DealsCarousel.
-   **Testing**: Uses Vitest and `@testing-library/vue`.

---

## Development Workflow

### Adding a New Feature (Example)
A typical workflow involves these steps, from database to UI:

1.  **Backend**:
    -   Define an **Entity** in `Commerce.Repositories`.
    -   Create an EF Core **Migration**.
    -   Implement the **Repository** for data access.
    -   Define **DTOs** and a **Service** for business logic.
    -   Expose functionality via a **Controller** in `Commerce.Api`.
2.  **Frontend**:
    -   Define TypeScript **Types** for the new data.
    -   Create an API **Service** to communicate with the backend.
    -   Build a **Composable** to manage state.
    -   Create **Components** and assemble them on a **Page**.

### Running Tests
-   **Frontend**:
    ```bash
    cd frontend
    npm run test
    npm run test:coverage # For coverage reports
    ```
-   **Backend**:
    ```bash
    # From the 'backend/tests/Commerce.UnitTests' directory
    dotnet test
    ```
    ```bash
    # From the 'backend/tests/Commerce.IntegrationTests' directory
    dotnet test
    ```

### Database Management
-   **Reset Database**: To completely wipe and restart the database:
    ```bash
    docker compose down -v
    docker compose up --build
    ```
-   **Connect via `psql`**:
    ```bash
    docker compose exec postgres psql -U commerce -d commerce_db
    ```
