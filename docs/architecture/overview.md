# Architecture Overview

## System Context (C4 Level 1)

```
┌─────────────────────────────────────────────────────────────────┐
│                         Users                                    │
│                    (Web Browsers)                                │
└──────────────────────────┬──────────────────────────────────────┘
                           │ HTTPS
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│                  Commerce Vue.NET System                         │
│  ┌─────────────────┐              ┌─────────────────┐           │
│  │    Frontend     │   HTTP/JSON  │    Backend      │           │
│  │   (Vue 3 SPA)   │◄────────────►│  (.NET 8 API)   │           │
│  │    :5173        │              │    :8080        │           │
│  └─────────────────┘              └────────┬────────┘           │
│                                            │                     │
│                          ┌─────────────────┼─────────────────┐  │
│                          │                 │                 │  │
│                          ▼                 ▼                 │  │
│                   ┌─────────────┐   ┌─────────────┐         │  │
│                   │  PostgreSQL │   │    MinIO    │         │  │
│                   │    :5432    │   │  :9000/9001 │         │  │
│                   └─────────────┘   └─────────────┘         │  │
└─────────────────────────────────────────────────────────────────┘
```

## Container Diagram (C4 Level 2)

### Frontend Container
- **Technology**: Vue 3 + TypeScript + Vuetify 3
- **Responsibilities**:
  - User interface rendering
  - Client-side routing (file-based)
  - State management (composables + minimal Pinia)
  - API communication via HTTP client
  - Internationalization (en, es)

### Backend Container
- **Technology**: .NET 8 + ASP.NET Core + EF Core
- **Responsibilities**:
  - REST API endpoints
  - Business logic processing
  - Data validation (FluentValidation)
  - Database access (EF Core)
  - File storage (MinIO via AWS S3 SDK)

### PostgreSQL Container
- **Technology**: PostgreSQL 16 Alpine
- **Data**: Products, Categories, CategoryLinks, ProductImages

### MinIO Container
- **Technology**: MinIO (S3-compatible)
- **Data**: Product images, static assets
- **Access**: Public download, authenticated upload

## Component Diagram (C4 Level 3)

### Backend Components

```
┌─────────────────────────────────────────────────────────────────┐
│                        Commerce.Api                              │
│  ┌──────────────────┐  ┌──────────────────┐                     │
│  │ ProductController│  │CategoryController│  ...                │
│  └────────┬─────────┘  └────────┬─────────┘                     │
│           │                     │                                │
└───────────┼─────────────────────┼────────────────────────────────┘
            │                     │
            ▼                     ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Commerce.Services                            │
│  ┌──────────────────┐  ┌──────────────────┐                     │
│  │  ProductService  │  │ CategoryService  │  ...                │
│  │     (DTOs)       │  │    (DTOs)        │                     │
│  └────────┬─────────┘  └────────┬─────────┘                     │
│           │                     │                                │
└───────────┼─────────────────────┼────────────────────────────────┘
            │                     │
            ▼                     ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Commerce.Repositories                          │
│  ┌──────────────────┐  ┌──────────────────┐                     │
│  │ ProductRepository│  │CategoryRepository│  ...                │
│  │   (Entities)     │  │   (Entities)     │                     │
│  └────────┬─────────┘  └────────┬─────────┘                     │
│           │                     │                                │
│           └──────────┬──────────┘                                │
│                      ▼                                           │
│           ┌──────────────────┐                                   │
│           │ CommerceDbContext│                                   │
│           └──────────────────┘                                   │
└─────────────────────────────────────────────────────────────────┘
```

### Frontend Components

```
┌─────────────────────────────────────────────────────────────────┐
│                           Pages                                  │
│  ┌─────────────────┐  ┌─────────────────┐                       │
│  │   ProductsPage  │  │  CategoriesPage │  ...                  │
│  └────────┬────────┘  └────────┬────────┘                       │
│           │                    │                                 │
└───────────┼────────────────────┼─────────────────────────────────┘
            │                    │
            ▼                    ▼
┌─────────────────────────────────────────────────────────────────┐
│                        Composables                               │
│  ┌─────────────────┐  ┌─────────────────┐                       │
│  │   useProducts   │  │  useCategories  │  ...                  │
│  │  (reactive state)│ │ (reactive state)│                       │
│  └────────┬────────┘  └────────┬────────┘                       │
│           │                    │                                 │
└───────────┼────────────────────┼─────────────────────────────────┘
            │                    │
            ▼                    ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Services                                 │
│  ┌─────────────────┐  ┌─────────────────┐                       │
│  │   productsApi   │  │   categoryApi   │  ...                  │
│  │  (ApiResult<T>) │  │  (ApiResult<T>) │                       │
│  └────────┬────────┘  └────────┬────────┘                       │
│           │                    │                                 │
│           └─────────┬──────────┘                                 │
│                     ▼                                            │
│           ┌─────────────────┐                                    │
│           │   lib/http.ts   │                                    │
│           └─────────────────┘                                    │
└─────────────────────────────────────────────────────────────────┘
```

## Data Flow

### Read Flow (GET Product)
```
Browser → Vue Page → useProducts() → productsApi.getProducts()
                                           │
                                           ▼ HTTP GET
                                    ProductController.Get()
                                           │
                                           ▼
                                    ProductService.GetAll()
                                           │
                                           ▼
                                    ProductRepository.GetAll()
                                           │
                                           ▼
                                    EF Core → PostgreSQL
```

### Write Flow (POST Product)
```
Browser → Form Submit → useProducts().create()
                              │
                              ▼
                       productsApi.create(dto)
                              │
                              ▼ HTTP POST
                       ProductController.Create()
                              │
                              ▼
                       ProductService.Create()
                              │
                              ├──────────────────────┐
                              ▼                      ▼
                       ProductRepository.Add()   StorageService
                              │                      │
                              ▼                      ▼
                       PostgreSQL               MinIO (images)
```

## Technology Stack

### Backend
| Layer | Technology |
|-------|------------|
| API | ASP.NET Core 8.0 |
| Business Logic | C# Services |
| Validation | FluentValidation |
| ORM | Entity Framework Core |
| Database | PostgreSQL 16 |
| Storage | MinIO (AWS S3 SDK) |
| Testing | xUnit, Moq, Testcontainers |

### Frontend
| Layer | Technology |
|-------|------------|
| Framework | Vue 3 (Composition API) |
| Language | TypeScript (strict) |
| UI Library | Vuetify 3 |
| State | Composables + Pinia (minimal) |
| Routing | Vue Router (file-based) |
| i18n | Vue i18n |
| Build | Vite |
| Testing | Vitest, Testing Library |

## Deployment Architecture

### Current (Development)
- Docker Compose with profiles
- `infra` profile: PostgreSQL, MinIO
- `app` profile: Backend, Frontend
- All on single host

### Future (Production) - TODO
- Container orchestration (Kubernetes or similar)
- Managed database (RDS, Cloud SQL)
- CDN for frontend assets
- Object storage (S3, Cloud Storage)
- Load balancing
- SSL/TLS termination
