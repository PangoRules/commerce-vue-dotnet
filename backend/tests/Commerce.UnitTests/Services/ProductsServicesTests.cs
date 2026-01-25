using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Services;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.UnitTests.Services;

public class ProductsServicesTests
{
    [Fact]
    public async Task GetProductByIdAsync_WhenRepoReturnsNull_ReturnsNull()
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            ProductById = null
        };
        var sut = new ProductsServices(repo);

        // Act
        var result = await sut.GetProductByIdAsync(productId: 123);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenRepoReturnsProduct_ReturnsMappedResponse()
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            ProductById = new Product
            {
                Id = 10,
                CategoryId = 2,
                Name = "Laptop",
                Description = "Lightweight laptop",
                Price = 1199.99m,
                StockQuantity = 20,
                IsActive = true
            }
        };
        var sut = new ProductsServices(repo);

        // Act
        var result = await sut.GetProductByIdAsync(productId: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result!.Id);
        Assert.Equal(2, result.CategoryId);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal("Lightweight laptop", result.Description);
        Assert.Equal(1199.99m, result.Price);
        Assert.Equal(20, result.StockQuantity);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetAllActiveProductsAsync_MapsAllProducts()
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            ActiveProducts = new()
            {
                new Product { Id = 1, CategoryId = 1, Name = "A", Price = 1m, StockQuantity = 1, IsActive = true },
                new Product { Id = 2, CategoryId = 1, Name = "B", Price = 2m, StockQuantity = 2, IsActive = true },
            }
        };
        var sut = new ProductsServices(repo);

        // Act
        var results = await sut.GetAllProductsAsync(new GetProductsQueryParams());

        // Assert
        Assert.Equal(2, results.Items.Count);
        Assert.Equal(1, results.Items[0].Id);
        Assert.Equal("A", results.Items[0].Name);
        Assert.Equal(2, results.Items[1].Id);
        Assert.Equal("B", results.Items[1].Name);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddProductAsync_ReturnsRepoResult(bool repoResult)
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            AddProductResult = repoResult
        };
        var sut = new ProductsServices(repo);

        var request = new CreateProductRequest
        {
            CategoryId = 1,
            Name = "New",
            Description = null,
            Price = 9.99m,
            StockQuantity = 5
        };

        // Act
        var result = await sut.AddProductAsync(request);

        // Assert
        Assert.Equal(repoResult, result);
        Assert.Same(request, repo.LastCreateRequest); // proves it passed through
    }

    [Fact]
    public async Task UpdateProductAsync_WhenRepoReturnsNull_ReturnsNull()
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            UpdatedProduct = null
        };
        var sut = new ProductsServices(repo);

        // Act
        var result = await sut.UpdateProductAsync(
            new UpdateProductRequest { CategoryId = 1, Name = "X", Description = null, Price = 1m, StockQuantity = 1 },
            productId: 123);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenRepoReturnsProduct_ReturnsMappedResponse()
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            UpdatedProduct = new Product
            {
                Id = 5,
                CategoryId = 2,
                Name = "Updated",
                Description = "Updated Desc",
                Price = 50m,
                StockQuantity = 7,
                IsActive = true
            }
        };
        var sut = new ProductsServices(repo);

        var request = new UpdateProductRequest
        {
            CategoryId = 2,
            Name = "Updated",
            Description = "Updated Desc",
            Price = 50m,
            StockQuantity = 7
        };

        // Act
        var result = await sut.UpdateProductAsync(request, productId: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("Updated", result.Name);
        Assert.Same(request, repo.LastUpdateRequest);
        Assert.Equal(5, repo.LastUpdateProductId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ToggleProductAsync_ReturnsRepoResult(bool repoResult)
    {
        // Arrange
        var repo = new FakeProductsRepository
        {
            ToggleResult = repoResult
        };
        var sut = new ProductsServices(repo);

        // Act
        var result = await sut.ToggleProductAsync(productId: 77);

        // Assert
        Assert.Equal(repoResult, result);
        Assert.Equal(77, repo.LastToggleProductId);
    }

    /// <summary>
    /// Minimal fake for unit tests
    /// </summary>
    private sealed class FakeProductsRepository : IProductsRepository
    {
        public Product? ProductById { get; set; }
        public List<Product> ActiveProducts { get; set; } = new();
        public List<Product> ActiveProductsByCategory { get; set; } = new();

        public bool AddProductResult { get; set; }
        public bool ToggleResult { get; set; }
        public Product? UpdatedProduct { get; set; }

        public CreateProductRequest? LastCreateRequest { get; private set; }
        public UpdateProductRequest? LastUpdateRequest { get; private set; }
        public int? LastUpdateProductId { get; private set; }
        public int? LastToggleProductId { get; private set; }

        public Task<Product?> GetProductByIdAsync(int productId) =>
            Task.FromResult(ProductById);

        public Task<List<Product>> GetAllActiveProductsByCategoryIdAsync(int categoryId) =>
            Task.FromResult(ActiveProductsByCategory);

        public Task<bool> AddProductAsync(CreateProductRequest product)
        {
            LastCreateRequest = product;
            return Task.FromResult(AddProductResult);
        }

        public Task<Product?> UpdateProductAsync(UpdateProductRequest product, int productId)
        {
            LastUpdateRequest = product;
            LastUpdateProductId = productId;
            return Task.FromResult(UpdatedProduct);
        }

        public Task<bool> ToggleProductAsync(int productId)
        {
            LastToggleProductId = productId;
            return Task.FromResult(ToggleResult);
        }

        public Task<PagedResult<Product>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct = default)
        {
            return Task.FromResult(new PagedResult<Product>(ActiveProducts, 1, 10, ActiveProducts.Count));
        }
    }
}