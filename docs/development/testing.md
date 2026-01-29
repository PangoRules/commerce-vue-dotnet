# Testing Strategy

## Overview

| Layer | Framework | Coverage Target |
|-------|-----------|-----------------|
| Backend Services | xUnit + Moq | 75%+ |
| Backend Integration | xUnit + Testcontainers | Critical paths |
| Frontend Composables | Vitest | 75%+ |
| Frontend Services | Vitest | 75%+ |
| Frontend Components | Vitest + Testing Library | Key interactions |

## Backend Testing

### Unit Tests (Commerce.Services.Tests)

**Location**: `backend/tests/Commerce.Services.Tests/`

**Pattern**: Test services with mocked repositories

```csharp
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsProducts()
    {
        // Arrange
        var products = new List<Product> { new() { Id = 1, Name = "Test" } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test", result.First().Name);
    }
}
```

**Commands**:

```bash
cd backend/tests/Commerce.Services.Tests

# Run all tests
dotnet test

# Run with verbosity
dotnet test --logger "console;verbosity=detailed"

# Filter tests
dotnet test --filter "ClassName=ProductServiceTests"

# Coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests (Commerce.Api.IntegrationTests)

**Location**: `backend/tests/Commerce.Api.IntegrationTests/`

**Pattern**: Test full API with Testcontainers for database

```csharp
public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();
    }
}
```

**Commands**:

```bash
cd backend/tests/Commerce.Api.IntegrationTests
dotnet test
```

## Frontend Testing

### Composable Tests

**Location**: Alongside source files (e.g., `useProducts.spec.ts`)

**Pattern**: Test reactive state and API integration

```typescript
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useProducts } from './useProducts'
import { productsApi } from '../services/productsApi'

vi.mock('../services/productsApi')

describe('useProducts', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('fetches products successfully', async () => {
    const mockProducts = [{ id: 1, name: 'Test Product' }]
    vi.mocked(productsApi.getProducts).mockResolvedValue({
      success: true,
      data: mockProducts
    })

    const { products, loading, error, fetchProducts } = useProducts()

    expect(loading.value).toBe(false)
    await fetchProducts()

    expect(products.value).toEqual(mockProducts)
    expect(error.value).toBeNull()
  })

  it('handles API errors', async () => {
    vi.mocked(productsApi.getProducts).mockResolvedValue({
      success: false,
      error: 'Network error'
    })

    const { error, fetchProducts } = useProducts()
    await fetchProducts()

    expect(error.value).toBe('Network error')
  })
})
```

### Service Tests

**Pattern**: Test API client methods

```typescript
import { describe, it, expect, vi } from 'vitest'
import { productsApi } from './productsApi'
import { http } from '../lib/http'

vi.mock('../lib/http')

describe('productsApi', () => {
  it('getProducts returns ApiResult on success', async () => {
    vi.mocked(http.get).mockResolvedValue({
      data: [{ id: 1, name: 'Product' }]
    })

    const result = await productsApi.getProducts()

    expect(result.success).toBe(true)
    if (result.success) {
      expect(result.data).toHaveLength(1)
    }
  })

  it('getProducts returns error on failure', async () => {
    vi.mocked(http.get).mockRejectedValue(new Error('Network error'))

    const result = await productsApi.getProducts()

    expect(result.success).toBe(false)
  })
})
```

### Component Tests

**Pattern**: Test user interactions with Testing Library

```typescript
import { describe, it, expect } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/vue'
import ProductCard from './ProductCard.vue'

describe('ProductCard', () => {
  it('displays product name', () => {
    render(ProductCard, {
      props: {
        product: { id: 1, name: 'Test Product', price: 99.99 }
      }
    })

    expect(screen.getByText('Test Product')).toBeInTheDocument()
  })

  it('emits click event', async () => {
    const { emitted } = render(ProductCard, {
      props: {
        product: { id: 1, name: 'Test', price: 99.99 }
      }
    })

    await fireEvent.click(screen.getByRole('button'))

    expect(emitted().click).toBeTruthy()
  })
})
```

### Commands

```bash
cd frontend

# Watch mode
npm run test

# Single run
npm run test:run

# With coverage
npm run test:coverage

# UI mode
npm run test:ui
```

## Test File Organization

```
backend/tests/
├── Commerce.Services.Tests/
│   ├── ProductServiceTests.cs
│   ├── CategoryServiceTests.cs
│   └── ...
└── Commerce.Api.IntegrationTests/
    ├── ProductsApiTests.cs
    └── ...

frontend/src/
├── composables/
│   ├── useProducts.ts
│   ├── useProducts.spec.ts      # Co-located
│   ├── useCategories.ts
│   └── useCategories.spec.ts
├── services/
│   ├── productsApi.ts
│   ├── productsApi.spec.ts      # Co-located
│   └── ...
└── tests/
    └── helpers/                  # Shared test utilities
        └── setup.ts
```

## Coverage Reports

### Backend

Coverage collected with `--collect:"XPlat Code Coverage"`, outputs to `TestResults/` directory.

### Frontend

Coverage with `npm run test:coverage`, outputs to `coverage/` directory.

View HTML report:

```bash
# After running coverage
open coverage/index.html
```

## Mocking Patterns

### Backend (Moq)

```csharp
var mock = new Mock<IProductRepository>();
mock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
mock.Verify(r => r.GetByIdAsync(1), Times.Once);
```

### Frontend (Vitest)

```typescript
// Mock module
vi.mock('../services/productsApi')

// Mock implementation
vi.mocked(productsApi.getProducts).mockResolvedValue({
  success: true,
  data: []
})

// Verify calls
expect(productsApi.getProducts).toHaveBeenCalledTimes(1)
```
