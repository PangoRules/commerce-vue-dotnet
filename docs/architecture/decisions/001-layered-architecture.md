# ADR-001: Layered Architecture

## Status

Accepted

## Date

2026-01-28

## Context

We needed an architecture that would:

- Separate concerns clearly between API, business logic, and data access
- Enable unit testing of business logic without database dependencies
- Support multiple developers working on different layers
- Provide clear boundaries for the e-commerce domain

## Decision

### Backend: Clean Architecture with 4 Layers

```
Commerce.Api → Commerce.Services → Commerce.Repositories → Commerce.Shared
```

1. **Commerce.Api**: Controllers, dependency injection, Swagger
2. **Commerce.Services**: Business logic, DTOs, service interfaces
3. **Commerce.Repositories**: EF Core, entities, data access
4. **Commerce.Shared**: Cross-cutting concerns, validation

Dependencies flow downward only. Lower layers have no knowledge of higher layers.

### Frontend: Layered with Composables

```
Pages → Components → Composables → Services → Types
```

1. **Pages**: File-based routing, page composition
2. **Components**: Presentational Vuetify components
3. **Composables**: Business logic, reactive state management
4. **Services**: HTTP client, API abstraction
5. **Types**: TypeScript interfaces for API contracts

## Consequences

### Positive

- **Testability**: Services can be unit tested with mocked repositories
- **Separation**: UI changes don't affect business logic
- **Maintainability**: Clear location for each type of code
- **Team scaling**: Developers can work on layers independently

### Negative

- **Boilerplate**: More files and interfaces than simpler architectures
- **Indirection**: Data flows through multiple layers
- **Learning curve**: New developers must understand layer responsibilities

### Neutral

- DTOs and Entities are separate, requiring mapping code
- Each feature touches multiple layers and directories

## Alternatives Considered

### 1. Monolithic Backend

Single project with all code. Rejected due to testing difficulty and coupling.

### 2. CQRS/Event Sourcing

Too complex for current scope. Could be adopted later for specific domains.

### 3. Frontend State in Pinia Only

Rejected in favor of composables for better testability and colocation of related logic.

## References

- Clean Architecture (Robert C. Martin)
- Vue 3 Composition API best practices
