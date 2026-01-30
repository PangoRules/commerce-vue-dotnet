# Deployment

## Current State: Docker Compose (Development)

The project uses Docker Compose for local development with two profiles:

### Profiles

| Profile | Services | Use Case |
|---------|----------|----------|
| `infra` | PostgreSQL, MinIO, minio-init | Database and storage |
| `app` | Backend, Frontend | Application services |

### Commands

```bash
# Start infrastructure only
docker compose --profile infra up -d

# Start full stack
docker compose --profile infra --profile app up

# Start with rebuild
docker compose --profile infra --profile app up --build

# Stop all
docker compose down

# Stop and remove volumes
docker compose down -v
```

### Service Configuration

| Service | Image | Port | Health Check |
|---------|-------|------|--------------|
| postgres | postgres:16-alpine | 5432 | pg_isready |
| minio | minio/minio:latest | 9000, 9001 | - |
| minio-init | minio/mc:latest | - | (init only) |
| backend | Custom Dockerfile | 8080 | - |
| frontend | Custom Dockerfile | 5173 | - |

### Volumes

| Volume | Purpose |
|--------|---------|
| commerce_pgdata | PostgreSQL data persistence |
| commerce_minio_data | MinIO object storage |

### Networks

All services connect via `commerce-net` bridge network.

## Environment Variables

See [Getting Started](../development/getting-started.md) for full list.

Key variables:
- `POSTGRES_*`: Database credentials
- `MINIO_*`: Object storage configuration
- `VITE_API_BASE_URL`: Frontend API endpoint
- `ConnectionStrings__DefaultConnection`: Backend database connection

## TODO: Production Deployment

### Container Orchestration
- [ ] Kubernetes manifests or Helm charts
- [ ] Service mesh (Istio/Linkerd) for traffic management
- [ ] Horizontal pod autoscaling
- [ ] Rolling update strategy

### Database
- [ ] Managed PostgreSQL (RDS, Cloud SQL, Azure Database)
- [ ] Connection pooling (PgBouncer)
- [ ] Automated backups
- [ ] Read replicas for scaling

### Object Storage
- [ ] AWS S3 / Google Cloud Storage / Azure Blob
- [ ] CDN for image delivery
- [ ] Lifecycle policies for old images

### Frontend
- [ ] Static build deployment (Nginx, CDN)
- [ ] Environment-specific builds
- [ ] Asset optimization and caching

### Backend
- [ ] Container registry (ECR, GCR, ACR)
- [ ] Health check endpoints
- [ ] Graceful shutdown handling
- [ ] Resource limits and requests

### Security
- [ ] SSL/TLS termination (ingress/load balancer)
- [ ] Secret management (Vault, AWS Secrets Manager)
- [ ] Network policies
- [ ] Pod security standards

### CI/CD
- [ ] GitHub Actions workflows
- [ ] Automated testing in pipeline
- [ ] Container image scanning
- [ ] Deployment automation

## Build Artifacts

### Backend Docker Build

```dockerfile
# backend/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Commerce.Api.dll"]
```

### Frontend Docker Build

```dockerfile
# frontend/Dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
```

## Rollback Strategy

### Current (Docker Compose)

```bash
# Rollback to previous image version
docker compose pull backend:previous-tag
docker compose up -d backend
```

### Production (TODO)

- Blue-green deployment
- Canary releases
- Database migration rollback procedures
