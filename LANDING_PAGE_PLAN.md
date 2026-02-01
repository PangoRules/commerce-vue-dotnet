# Dynamic E-Commerce Landing Page Implementation Plan

This document outlines the step-by-step plan to create a dynamic, backend-driven landing page for the Commerce Vue.NET project.

## Overall Strategy

We will adopt a **backend-driven UI** approach. The backend will define which sections are active and provide the data for them in a single API call. The frontend's role is to request this homepage configuration and render the corresponding components based on the response.

This plan is broken down into three main phases: **Backend (.NET)**, **Frontend (Vue)**, and **Data Seeding**.

---

## Phase 1: Backend (.NET)

### 1. Define/Update Database Entities

We need to adjust our database schema to support these new concepts.

#### A. Featured Categories
In `backend/src/Commerce.Repositories/Entities/Category.cs`, add a boolean flag.

```csharp
// In Commerce.Repositories/Entities/Category.cs
public class Category
{
    // ... existing properties
    public bool IsFeatured { get; set; } = false; // Add this line
}
```

#### B. Deals/Sales
In `backend/src/Commerce.Repositories/Entities/Product.cs`, add a nullable `SalePrice`. If the price has a value, the product is on sale.

```csharp
// In Commerce.Repositories/Entities/Product.cs
public class Product
{
    // ... existing properties
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; } // Add this line
}
```
**Note on Best Sellers:** For this implementation, we can consider any product with a `SalePrice` as a "Best Seller" to simplify the data model. The logic can be made more sophisticated later (e.g., based on sales volume).

#### C. Dynamic Homepage Sections
Create a new entity to define the homepage sections themselves. This is the core of the admin-configurable feature.

Create a new file: `backend/src/Commerce.Repositories/Entities/HomepageSection.cs`

```csharp
// In Commerce.Repositories/Entities/HomepageSection.cs
using System.ComponentModel.DataAnnotations;

namespace Commerce.Repositories.Entities;

public class HomepageSection
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty; // e.g., "FeaturedCategories", "BestSellers", "Deals"

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}
```

Finally, add the new `DbSet` to your `CommerceDbContext` in `backend/src/Commerce.Repositories/Context/CommerceDbContext.cs`.

```csharp
// In Commerce.Repositories/Context/CommerceDbContext.cs
public DbSet<HomepageSection> HomepageSections { get; set; }
```

### 2. Create EF Core Migration

From the `backend/src/Commerce.Api` directory, run the following commands in your terminal:

```bash
# To create the migration script
dotnet ef migrations add AddHomepageEntities -p ../Commerce.Repositories

# To apply the migration to your database
dotnet ef database update -p ../Commerce.Repositories
```

### 3. Implement Services to Fetch Data

- **Update `ProductService.cs`**: Create a method `GetDeals()` that returns products where `SalePrice` is not null.
- **Update `CategoryService.cs`**: Create a method `GetFeaturedCategories()` that returns categories where `IsFeatured` is true.
- **Create `HomepageService.cs`**: This new service will be responsible for fetching the active `HomepageSection` entities and then calling the other services to get the data for those sections.

### 4. Create the Homepage API Controller

- **Define DTOs**: In `Commerce.Shared/Responses`, create a `HomepageDto.cs` to structure the response.
  ```csharp
  // Example structure for the response DTO
  public class HomepageDto
  {
      public List<SectionDto> Sections { get; set; }
  }

  public class SectionDto
  {
      public string Name { get; set; } // "FeaturedCategories"
      public int DisplayOrder { get; set; }
      public object Data { get; set; } // Will hold a list of ProductDto or CategoryDto
  }
  ```
- **Create `HomepageController.cs`**: In `backend/src/Commerce.Api/Controllers`, create a new controller with a single, efficient endpoint: `GET /api/homepage`. This endpoint will use the `HomepageService` to build and return the `HomepageDto`.

---

## Phase 2: Frontend (Vue)

### 5. Define Frontend Types & Services

- **Create `frontend/src/types/homepage.ts`**: Define TypeScript interfaces that match the DTOs from the backend API.
- **Create `frontend/src/services/homepageService.ts`**: Add a function that makes a GET request to the `/api/homepage` endpoint.

### 6. Create a `useHomepage` Composable

- **Create `frontend/src/composables/useHomepage.ts`**: This composable will be the primary state management for the landing page.
- It will call the `homepageService`, store all the data in reactive state (`ref`, `reactive`), and expose the data, loading state, and any errors.

### 7. Build the Homepage Components

- **Parent Page (`frontend/src/pages/index.vue`)**: This new page will be the landing page. It will use the `useHomepage` composable and dynamically render the section components based on the data received from the API. Use `v-if` or a dynamic `<component :is="...">` to loop through the sections.

- **Child Section Components**: Create these reusable, "dumb" components in `frontend/src/components/homepage/`:
    - `HeroSection.vue` (can be a static hero banner)
    - `FeaturedCategories.vue`
    - `DealsCarousel.vue`
    - `BestSellers.vue`
  These components should receive their data via props.

### 8. Update Vue Router

Your project uses file-based routing (`unplugin-vue-router`). By creating `pages/index.vue`, it will automatically become the new root URL (`/`). If you have an existing `index.vue`, you may need to rename it (e.g., to `pages/products.vue`).

---

## Phase 3: Data Seeding

To view and test the new homepage, you will need to seed the database with initial data. You can do this by extending your EF Core data seeding logic or by connecting to the database directly and running SQL `INSERT` statements to:

1.  **Create rows in the `HomepageSections` table** (e.g., 'Best Sellers', 'Deals', 'Featured Categories') and set their `IsActive` and `DisplayOrder` properties.
2.  **Update a few categories** to set `IsFeatured = true`.
3.  **Update a few products** to give them a `SalePrice`.

After completing this plan, the final step would be to create a simple admin interface where a non-developer can manage these settings dynamically.
