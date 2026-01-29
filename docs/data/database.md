# Database Documentation

## Overview

| Property | Value |
|----------|-------|
| Database | PostgreSQL 16 Alpine |
| ORM | Entity Framework Core |
| Port | 5432 |
| Default Database | commerce_db |

## Connection

### Development (Local)

```
Host=localhost;Port=5432;Database=commerce_db;Username=commerce;Password=commerce_password
```

### Docker Internal

```
Host=postgres;Port=5432;Database=commerce_db;Username=commerce;Password=commerce_password
```

### Configuration

Environment variables in `.env`:
```bash
POSTGRES_USER=commerce
POSTGRES_PASSWORD=commerce_password
POSTGRES_DB=commerce_db
POSTGRES_PORT=5432
```

Backend connection string in `appsettings.json` or via environment variable:
```
ConnectionStrings__DefaultConnection=Host=...
```

## Schema

### Product

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| Name | varchar | NOT NULL |
| Description | text | NULLABLE |
| Price | decimal(18,2) | NOT NULL |
| CategoryId | int | FK → Category.Id |
| CreatedAt | timestamp | NOT NULL, default now |
| UpdatedAt | timestamp | NULLABLE |

### ProductImage

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| ProductId | int | FK → Product.Id |
| Url | varchar | NOT NULL |
| IsPrimary | boolean | NOT NULL, default false |
| CreatedAt | timestamp | NOT NULL |

### Category

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| Name | varchar | NOT NULL |
| Description | text | NULLABLE |
| CreatedAt | timestamp | NOT NULL |

### CategoryLink

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| ParentCategoryId | int | FK → Category.Id |
| ChildCategoryId | int | FK → Category.Id |

## Entity Relationships

```
Category (1) ←─── CategoryLink ───→ (1) Category
    │                                   (parent-child hierarchy)
    │
    └──── (1) ←─────────────────── (*) Product
                                        │
                                        └── (1) ←─── (*) ProductImage
```

## EF Core Setup

### DbContext Location

`backend/src/Commerce.Repositories/CommerceDbContext.cs`

### Entity Configurations

`backend/src/Commerce.Repositories/Configurations/`

- `ProductConfiguration.cs`
- `ProductImageConfiguration.cs`
- `CategoryConfiguration.cs`
- `CategoryLinkConfiguration.cs`

### Configuration Example

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId);
    }
}
```

## Migrations

### Location

`backend/src/Commerce.Repositories/Migrations/`

### Commands

```bash
cd backend/src/Commerce.Api

# Create migration
dotnet ef migrations add <Name> --project ../Commerce.Repositories

# Apply migrations
dotnet ef database update --project ../Commerce.Repositories

# List migrations
dotnet ef migrations list --project ../Commerce.Repositories

# Generate SQL script
dotnet ef migrations script --project ../Commerce.Repositories

# Revert to specific migration
dotnet ef database update <MigrationName> --project ../Commerce.Repositories

# Remove last migration (if not applied)
dotnet ef migrations remove --project ../Commerce.Repositories
```

### Migration Naming

Use descriptive names:
- `AddProductTable`
- `AddCategoryLink`
- `UpdateProductAddDescription`
- `RemoveObsoleteColumn`

## Seeding

### Development Data

Seed data can be added in:
1. Migration files (permanent seed data)
2. `CommerceDbContext.OnModelCreating()` using `HasData()`
3. Separate seeding script

### Example HasData Seeding

```csharp
// In CategoryConfiguration.cs
builder.HasData(
    new Category { Id = 1, Name = "Electronics", CreatedAt = DateTime.UtcNow },
    new Category { Id = 2, Name = "Clothing", CreatedAt = DateTime.UtcNow }
);
```

## Database Access

### Repository Pattern

All database access goes through repositories:

```csharp
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}
```

### Direct Database Access (Development Only)

```bash
# Via Docker
docker compose exec postgres psql -U commerce -d commerce_db

# Common queries
\dt                    # List tables
\d+ products           # Describe table
SELECT * FROM "Products" LIMIT 10;
```

## Backup & Restore (TODO)

### Backup

```bash
docker compose exec postgres pg_dump -U commerce commerce_db > backup.sql
```

### Restore

```bash
docker compose exec -T postgres psql -U commerce commerce_db < backup.sql
```

## Performance Considerations

### Indexes

Add indexes for frequently queried columns:

```csharp
builder.HasIndex(p => p.CategoryId);
builder.HasIndex(p => p.Name);
```

### Query Optimization

- Use `AsNoTracking()` for read-only queries
- Use `Include()` for eager loading related entities
- Avoid N+1 queries with proper loading strategies

```csharp
// Good: Eager load related data
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Images)
    .AsNoTracking()
    .ToListAsync();
```
