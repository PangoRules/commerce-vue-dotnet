# Commerce App — Vue 3 + .NET 8 (Dockerized Development Environment)

This repository contains a Docker-based local development environment composed of a Vue 3 frontend and an ASP.NET Core backend. The setup is designed to allow developers to spin up the entire stack with a single command while keeping the frontend and backend loosely coupled through environment-based configuration.

---

## Technology Stack

Frontend

- Vue 3
- TypeScript
- Vite
- Vuetify 3
- Native Fetch API with a reusable typed HTTP client

Backend

- ASP.NET Core (.NET 8)
- Minimal hosting model
- Swagger (enabled in Development only)
- CORS configured for local development

Infrastructure

- Docker
- Docker Compose
- Per-service Dockerfiles and .dockerignore files

---

## Repository Structure

```
.
├─ frontend/
│ ├─ Dockerfile
│ ├─ .dockerignore
│ └─ src/
│ ├─ lib/
│ │ └─ http.ts
│ ├─ services/
│ │ └─ apiClient.ts
│ ├─ composables/
│ └─ ...
├─ backend/
│ ├─ Dockerfile
│ ├─ .dockerignore
│ ├─ Commerce.sln
│ └─ src/
│ │ └─ Commerce.Api/
│ │ └─ Commerce.Repositories/
│ │ └─ Commerce.Services/
│ │ └─ Commerce.Shared/
│ └─ tests/
│   └─ Commerce.UnitTests/
├─ docker-compose.yml
└─ README.md
```

---

## Running the Application (Recommended)

Prerequisites

- Docker
- Docker Compose v2 or newer

From the repository root, run:

docker compose up --build

This command builds the frontend and backend images and starts both services on a shared Docker network.

Access points

- Frontend: http://localhost:5173
- Backend API: http://localhost:8080
- Swagger UI (Development only): http://localhost:8080/swagger
- Health endpoint: http://localhost:8080/api/health

---

## Running Services Individually

Backend only

```
cd backend
docker build -t commerce-backend-dev .
docker run --rm -p 8080:8080 commerce-backend-dev
```

Frontend only

```
cd frontend
docker build -t commerce-frontend-dev .
docker run --rm -p 5173:5173 commerce-frontend-dev
```

When running services independently, ensure the frontend API base URL is configured to point to the backend at http://localhost:8080.

---

## Frontend to Backend Communication

The frontend uses an environment-based API base URL provided at runtime:

```
VITE_API_BASE_URL
```

This value is injected via Docker Compose and consumed by a reusable HTTP client. All API interactions are routed through this client, which provides consistent typing and error handling for success and failure responses.

A healthcheck composable is included to verify connectivity between the frontend and backend during development.

---

## CORS Configuration

The backend explicitly allows cross-origin requests from the frontend development server:

http://localhost:5173

This is required because the frontend and backend run on different ports during local development.

---

## Docker Notes

- Each service has its own Dockerfile and .dockerignore
- .dockerignore files prevent unnecessary files (node_modules, bin, obj, etc.) from being included in the build context
- Docker images are intended for local development and are not production-hardened

---

## Development Notes

- Swagger is disabled in Production by default
- HTTPS redirection is not required for local Docker development
- The health endpoint is intended for smoke testing and development validation

---

## Common Commands

Build images manually

```
docker build -t commerce-backend-dev backend
docker build -t commerce-frontend-dev frontend
```

Run the full stack

```
docker compose up --build
```

Stop all services

```
docker compose down
```

---

## Future Improvements

- Add database service to Docker Compose
- Environment-specific compose configurations
- Production-ready Docker images
- Authentication and authorization support
