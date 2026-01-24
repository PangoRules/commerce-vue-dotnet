# Commerce App — Vue 3 + .NET 8 (Dockerized Development Environment)

This repository contains a Docker-based local development environment composed of a Vue 3 frontend, an ASP.NET Core backend, and a PostgreSQL database. The goal is to allow developers to spin up the full stack—or individual services—with minimal setup using Docker and Docker Compose.

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

Database

- PostgreSQL 16 (Dockerized)
- Persistent storage via Docker volume

Infrastructure

- Docker
- Docker Compose
- Per-service Dockerfiles and .dockerignore files

---

## Repository Structure

```
.
├─ frontend/
│  ├─ Dockerfile
│  ├─ .dockerignore
│  └─ src/
│     ├─ lib/
│     │  └─ http.ts
│     ├─ services/
│     │  └─ apiClient.ts
│     ├─ composables/
│     └─ ...
├─ backend/
│  ├─ Dockerfile
│  ├─ .dockerignore
│  ├─ Commerce.sln
│  └─ src/
│     ├─ Commerce.Api/
│     ├─ Commerce.Repositories/
│     ├─ Commerce.Services/
│     └─ Commerce.Shared/
│  └─ tests/
│     └─ Commerce.UnitTests/
├─ docker-compose.yml
├─ .env.example
└─ README.md
```

---

## Environment Variables

This repository includes a `.env.example` file that documents all environment variables required to run the application locally.

The `.env.example` file is provided for reference only and should not be modified directly. Each developer should create their own `.env` file based on it.

### Setup

```
cp .env.example .env
```

Update values in `.env` as needed. The `.env` file is ignored by Git and must not be committed.

---

## Running the Full Stack (Recommended)

### Prerequisites

- Docker
- Docker Compose v2 or newer

From the repository root:

```
docker compose up --build
```

This command builds the frontend and backend images, starts frontend, backend, and PostgreSQL, and creates a persistent database volume if one does not already exist.

### Access Points

- Frontend: [http://localhost:5173](http://localhost:5173)
- Backend API: [http://localhost:8080](http://localhost:8080)
- Swagger UI (Development only): [http://localhost:8080/swagger](http://localhost:8080/swagger)
- Health endpoint: [http://localhost:8080/api/health](http://localhost:8080/api/health)
- Db endpoint: [http://localhost:8080/api/health/db](http://localhost:8080/api/health/db)

---

## Running Services Independently

### Database Only (PostgreSQL)

Start only the database:

```
docker compose up -d postgres
```

PostgreSQL will be available on `localhost:5432`.

Stop the database:

```
docker compose stop postgres
```

---

### Backend Only (Docker)

```
cd backend
docker build -t commerce-backend-dev .
docker run --rm -p 8080:8080 commerce-backend-dev
```

Backend will be available at:

```
http://localhost:8080
```

Ensure PostgreSQL is running and reachable when running the backend independently.

---

### Frontend Only (Docker)

```
cd frontend
docker build -t commerce-frontend-dev .
docker run --rm -p 5173:5173 commerce-frontend-dev
```

Frontend will be available at:

```
http://localhost:5173
```

When running the frontend independently, ensure the API base URL points to `http://localhost:8080`.

---

## Database (PostgreSQL)

PostgreSQL runs as a Docker service and is intended for local development use.

### Default Configuration

- Image: postgres:16-alpine
- Database: commerce_db
- Username: commerce
- Password: commerce_password
- Host (from host machine): localhost
- Port: 5432
- Persistence: Docker named volume

---

### Connecting via psql (Host)

Install the PostgreSQL client:

```
sudo dnf install postgresql
```

Connect:

```
psql -h localhost -p 5432 -U commerce -d commerce_db
```

---

### Connecting via psql (Inside Container)

```
docker compose exec postgres psql -U commerce -d commerce_db
```

---

### Connecting via a GUI (DBeaver, pgAdmin, etc.)

Use the following settings:

- Host: localhost
- Port: 5432
- Database: commerce_db
- Username: commerce
- Password: commerce_password

---

### Backend Database Connection

Inside Docker, the backend connects to PostgreSQL using Docker’s internal DNS.

Example connection string:

```
Host=postgres;Port=5432;Database=commerce_db;Username=commerce;Password=commerce_password
```

---

### Resetting the Database (Dev Only)

To fully reset the local database and reapply all migrations and seed data:

```bash
docker compose down -v
#This runs all the items to be tested production like
docker compose up --build
#This only builds the items
docker compose build
#this so database gets initialized on the background ready to use dockerized
docker compose up -d postgres
```

This will:

- Stop all services
- Delete the PostgreSQL Docker volume
- Recreate a fresh database
- Automatically apply EF Core migrations
- Reinsert seed data (categories, products, etc.)

---

## Frontend to Backend Communication

The frontend uses an environment-based API base URL:

```
VITE_API_BASE_URL
```

This value is injected via Docker Compose and consumed by a reusable HTTP client. All API calls are routed through this client to provide consistent typing and centralized error handling.

A healthcheck composable is included to validate frontend-to-backend connectivity.

---

## CORS Configuration

The backend allows cross-origin requests from:

```
http://localhost:5173
```

This is required because the frontend and backend run on different ports during local development.

---

## Docker Notes

- Each service has its own Dockerfile and .dockerignore
- .dockerignore files reduce build context size and improve build performance
- Docker images are intended for local development only

---

## Common Commands

Build images manually:

```
docker build -t commerce-backend-dev backend
docker build -t commerce-frontend-dev frontend
```

Run the full stack:

```
docker compose up --build
```

Stop all services:

```
docker compose down
```

---

## Future Improvements

- Environment-specific Docker Compose configurations
- Database migrations and seeding
- Production-ready Docker images
- Authentication and authorization support
