# Getting Started

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| Docker | 24+ | Container runtime |
| Docker Compose | 2.20+ | Multi-container orchestration |
| Node.js | 20+ | Frontend development |
| .NET SDK | 8.0 | Backend development |
| Git | 2.40+ | Version control |

## Quick Start (Docker)

The fastest way to run the full stack:

```bash
# Clone the repository
git clone <repository-url>
cd commerce-vue-dotnet

# Copy environment file
cp .env.example .env

# Start infrastructure (PostgreSQL + MinIO)
docker compose --profile infra up -d

# Start applications (optional, for development use local instead)
docker compose --profile infra --profile app up
```

Access points:
- Frontend: http://localhost:5173
- Backend API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger
- MinIO Console: http://localhost:9001

## Local Development Setup

For active development, run infrastructure in Docker and apps locally:

### 1. Start Infrastructure

```bash
docker compose --profile infra up -d
```

### 2. Backend Setup

```bash
cd backend/src/Commerce.Api

# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project ../Commerce.Repositories

# Start the API
dotnet run
# Or with hot reload:
dotnet watch run
```

Backend runs at: http://localhost:8080

### 3. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev
```

Frontend runs at: http://localhost:5173

## Environment Variables

### .env File (Root)

```bash
# PostgreSQL
POSTGRES_USER=commerce
POSTGRES_PASSWORD=commerce_password
POSTGRES_DB=commerce_db
POSTGRES_PORT=5432

# MinIO
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin
MINIO_ENDPOINT=localhost:9000
MINIO_ACCESS_KEY=minioadmin
MINIO_SECRET_KEY=minioadmin
MINIO_BUCKET=products
MINIO_REGION=us-east-1
MINIO_USE_SSL=false
MINIO_PUBLIC_BASE_URL=http://localhost:9000/products

# Frontend
VITE_API_BASE_URL=http://localhost:8080
```

### Backend Configuration

Configuration in `backend/src/Commerce.Api/appsettings.json` is overridden by environment variables in Docker Compose.

For local development, create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=commerce_db;Username=commerce;Password=commerce_password"
  },
  "Storage": {
    "Provider": "Minio",
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "Bucket": "products",
    "Region": "us-east-1",
    "UseSsl": false,
    "PublicBaseUrl": "http://localhost:9000/products"
  }
}
```

## Verify Installation

### 1. Database Connection

```bash
# Check PostgreSQL is running
docker compose ps postgres

# Connect with psql (optional)
docker compose exec postgres psql -U commerce -d commerce_db
```

### 2. MinIO Connection

```bash
# Check MinIO is running
docker compose ps minio

# Access console at http://localhost:9001
# Default credentials: minioadmin / minioadmin
```

### 3. Backend Health

```bash
# Should return Swagger UI
curl http://localhost:8080/swagger/index.html
```

### 4. Frontend Build

```bash
cd frontend
npm run build  # Should complete without errors
npm run test   # Should pass all tests
```

## Troubleshooting

### Port Conflicts

If ports are in use, modify `.env`:

```bash
POSTGRES_PORT=5433
# Update VITE_API_BASE_URL if changing backend port
```

### Database Migration Errors

```bash
cd backend/src/Commerce.Api

# Check current migrations
dotnet ef migrations list --project ../Commerce.Repositories

# Force update
dotnet ef database update --project ../Commerce.Repositories
```

### Docker Cleanup

```bash
# Stop all containers
docker compose down

# Remove volumes (WARNING: deletes data)
docker compose down -v

# Rebuild images
docker compose build --no-cache
```

### Node Modules Issues

```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
```
