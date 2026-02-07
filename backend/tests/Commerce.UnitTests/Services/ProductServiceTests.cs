using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Services;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.UnitTests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProductByIdAsync_WhenRepoReturnsNull_ReturnsNull()
    {
        var productsRepo = new FakeProductsRepository { ProductById = null };
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var result = await sut.GetProductByIdAsync(productId: 123);

        Assert.Null(result);
        Assert.Equal(123, productsRepo.LastGetByIdProductId);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenRepoReturnsProduct_ReturnsMappedResponse()
    {
        var productsRepo = new FakeProductsRepository
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
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var result = await sut.GetProductByIdAsync(productId: 10);

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
    public async Task GetProductByIdAsync_WithLanguage_ReturnsTranslatedResponse()
    {
        var productsRepo = new FakeProductsRepository
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
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository
        {
            SingleTranslation = new EntityTranslation
            {
                Id = 1,
                EntityType = TranslatableEntityType.Product,
                EntityId = 10,
                LanguageCode = "es",
                Name = "Portátil",
                Description = "Portátil ligero"
            }
        };

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var result = await sut.GetProductByIdAsync(productId: 10, language: "es");

        Assert.NotNull(result);
        Assert.Equal("Portátil", result!.Name);
        Assert.Equal("Portátil ligero", result.Description);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithEnglishLanguage_SkipsTranslationLookup()
    {
        var productsRepo = new FakeProductsRepository
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
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var result = await sut.GetProductByIdAsync(productId: 10, language: "en");

        Assert.NotNull(result);
        Assert.Equal("Laptop", result!.Name);
        Assert.False(translationsRepo.GetTranslationCalled);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNullLanguage_SkipsTranslationLookup()
    {
        var productsRepo = new FakeProductsRepository
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
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var result = await sut.GetProductByIdAsync(productId: 10);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result!.Name);
        Assert.False(translationsRepo.GetTranslationCalled);
    }

    [Fact]
    public async Task GetAllProductsAsync_MapsProducts_AndPassesQueryParams()
    {
        var productsRepo = new FakeProductsRepository
        {
            PagedProducts = new PagedResult<Product>(
                Items:
                [
                    new Product { Id = 1, CategoryId = 1, Name = "A", Price = 1m, StockQuantity = 1, IsActive = true },
                    new Product { Id = 2, CategoryId = 1, Name = "B", Price = 2m, StockQuantity = 2, IsActive = true },
                ],
                Page: 1,
                PageSize: 10,
                TotalCount: 2)
        };
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);
        var queryParams = new GetProductsQueryParams { Page = 1, PageSize = 10 };

        var results = await sut.GetAllProductsAsync(queryParams);

        Assert.Equal(2, results.Items.Count);
        Assert.Equal(1, results.Items[0].Id);
        Assert.Equal("A", results.Items[0].Name);
        Assert.Equal(2, results.Items[1].Id);
        Assert.Equal("B", results.Items[1].Name);

        Assert.Equal(1, results.Page);
        Assert.Equal(10, results.PageSize);
        Assert.Equal(2, results.TotalCount);

        Assert.Same(queryParams, productsRepo.LastQueryParams);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithLanguage_ReturnsTranslatedResponses()
    {
        var productsRepo = new FakeProductsRepository
        {
            PagedProducts = new PagedResult<Product>(
                Items:
                [
                    new Product { Id = 1, CategoryId = 1, Name = "Laptop", Price = 1m, StockQuantity = 1, IsActive = true },
                    new Product { Id = 2, CategoryId = 1, Name = "Phone", Price = 2m, StockQuantity = 2, IsActive = true },
                ],
                Page: 1,
                PageSize: 10,
                TotalCount: 2)
        };
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository
        {
            BulkTranslations =
            [
                new EntityTranslation { Id = 1, EntityType = TranslatableEntityType.Product, EntityId = 1, LanguageCode = "es", Name = "Portátil" },
                new EntityTranslation { Id = 2, EntityType = TranslatableEntityType.Product, EntityId = 2, LanguageCode = "es", Name = "Teléfono" },
            ]
        };

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);
        var queryParams = new GetProductsQueryParams { Page = 1, PageSize = 10 };

        var results = await sut.GetAllProductsAsync(queryParams, language: "es");

        Assert.Equal("Portátil", results.Items[0].Name);
        Assert.Equal("Teléfono", results.Items[1].Name);
    }

    [Fact]
    public async Task AddProductAsync_WhenCategoryNotFound_ReturnsNotFound_AndDoesNotCallRepo()
    {
        var productsRepo = new FakeProductsRepository();
        var categoriesRepo = new FakeCategoryRepository { CategoryById = null };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new CreateProductRequest
        {
            CategoryId = 999,
            Name = "New",
            Description = null,
            Price = 9.99m,
            StockQuantity = 5
        };

        var (result, productId) = await sut.AddProductAsync(request);

        Assert.Equal(DbResultOption.NotFound, result);
        Assert.Equal(0, productId);

        Assert.Equal(999, categoriesRepo.LastGetByIdCategoryId);
        Assert.Null(productsRepo.LastCreateRequest); // proves it didn't call repo
    }

    [Fact]
    public async Task AddProductAsync_WhenCategoryInactive_ReturnsInvalid_AndDoesNotCallRepo()
    {
        var productsRepo = new FakeProductsRepository();
        var categoriesRepo = new FakeCategoryRepository
        {
            CategoryById = new Category { Id = 1, Name = "Cat", IsActive = false }
        };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new CreateProductRequest
        {
            CategoryId = 1,
            Name = "New",
            Description = null,
            Price = 9.99m,
            StockQuantity = 5
        };

        var (result, productId) = await sut.AddProductAsync(request);

        Assert.Equal(DbResultOption.Invalid, result);
        Assert.Equal(0, productId);
        Assert.Null(productsRepo.LastCreateRequest);
    }

    [Fact]
    public async Task AddProductAsync_WhenCategoryActive_ReturnsRepoResult_AndPassesRequestThrough()
    {
        var productsRepo = new FakeProductsRepository
        {
            AddResult = (DbResultOption.Success, 1234)
        };
        var categoriesRepo = new FakeCategoryRepository
        {
            CategoryById = new Category { Id = 1, Name = "Cat", IsActive = true }
        };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new CreateProductRequest
        {
            CategoryId = 1,
            Name = "New",
            Description = null,
            Price = 9.99m,
            StockQuantity = 5
        };

        var (result, productId) = await sut.AddProductAsync(request);

        Assert.Equal(DbResultOption.Success, result);
        Assert.Equal(1234, productId);

        Assert.Same(request, productsRepo.LastCreateRequest);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenCategoryNotFound_ReturnsNotFound_AndDoesNotCallRepo()
    {
        var productsRepo = new FakeProductsRepository();
        var categoriesRepo = new FakeCategoryRepository { CategoryById = null };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new UpdateProductRequest
        {
            CategoryId = 999,
            Name = "X",
            Description = null,
            Price = 1m,
            StockQuantity = 1
        };

        var (result, updated) = await sut.UpdateProductAsync(request, productId: 123);

        Assert.Equal(DbResultOption.NotFound, result);
        Assert.Null(updated);
        Assert.Null(productsRepo.LastUpdateRequest);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenCategoryInactive_ReturnsInvalid_AndDoesNotCallRepo()
    {
        var productsRepo = new FakeProductsRepository();
        var categoriesRepo = new FakeCategoryRepository
        {
            CategoryById = new Category { Id = 2, Name = "Cat", IsActive = false }
        };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new UpdateProductRequest
        {
            CategoryId = 2,
            Name = "X",
            Description = null,
            Price = 1m,
            StockQuantity = 1
        };

        var (result, updated) = await sut.UpdateProductAsync(request, productId: 123);

        Assert.Equal(DbResultOption.Invalid, result);
        Assert.Null(updated);
        Assert.Null(productsRepo.LastUpdateRequest);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenRepoReturnsNotFound_ReturnsNotFound()
    {
        var productsRepo = new FakeProductsRepository
        {
            UpdateResult = (DbResultOption.NotFound, null)
        };
        var categoriesRepo = new FakeCategoryRepository
        {
            CategoryById = new Category { Id = 2, Name = "Cat", IsActive = true }
        };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new UpdateProductRequest
        {
            CategoryId = 2,
            Name = "Updated",
            Description = "Updated Desc",
            Price = 50m,
            StockQuantity = 7
        };

        var (result, updated) = await sut.UpdateProductAsync(request, productId: 5);

        Assert.Equal(DbResultOption.NotFound, result);
        Assert.Null(updated);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenRepoReturnsSuccess_MapsResponse_AndPassesArgsThrough()
    {
        var productsRepo = new FakeProductsRepository
        {
            UpdateResult = (DbResultOption.Success, new Product
            {
                Id = 5,
                CategoryId = 2,
                Name = "Updated",
                Description = "Updated Desc",
                Price = 50m,
                StockQuantity = 7,
                IsActive = true
            })
        };
        var categoriesRepo = new FakeCategoryRepository
        {
            CategoryById = new Category { Id = 2, Name = "Cat", IsActive = true }
        };
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var request = new UpdateProductRequest
        {
            CategoryId = 2,
            Name = "Updated",
            Description = "Updated Desc",
            Price = 50m,
            StockQuantity = 7
        };

        var (result, updated) = await sut.UpdateProductAsync(request, productId: 5);

        Assert.Equal(DbResultOption.Success, result);
        Assert.NotNull(updated);
        Assert.Equal(5, updated!.Id);
        Assert.Equal("Updated", updated.Name);

        Assert.Same(request, productsRepo.LastUpdateRequest);
        Assert.Equal(5, productsRepo.LastUpdateProductId);
    }

    [Fact]
    public async Task ToggleProductAsync_ReturnsRepoResult_AndPassesIdThrough()
    {
        var productsRepo = new FakeProductsRepository { ToggleResult = DbResultOption.Success };
        var categoriesRepo = new FakeCategoryRepository();
        var imagesRepo = new FakeImageAssetRepository();
        var translationsRepo = new FakeEntityTranslationRepository();

        var sut = new ProductService(productsRepo, categoriesRepo, imagesRepo, translationsRepo);

        var result = await sut.ToggleProductAsync(productId: 77);

        Assert.Equal(DbResultOption.Success, result);
        Assert.Equal(77, productsRepo.LastToggleProductId);
    }

    // ----------------------------
    // Fakes
    // ----------------------------

    private sealed class FakeProductsRepository : IProductRepository
    {
        public Product? ProductById { get; set; }
        public PagedResult<Product> PagedProducts { get; set; } =
            new PagedResult<Product>(Items: [], Page: 1, PageSize: 10, TotalCount: 0);

        public (DbResultOption Result, int ProductId) AddResult { get; set; } = (DbResultOption.Success, 1);
        public (DbResultOption Result, Product? Product) UpdateResult { get; set; } = (DbResultOption.Success, null);
        public DbResultOption ToggleResult { get; set; } = DbResultOption.Success;

        // captured
        public int? LastGetByIdProductId { get; private set; }
        public GetProductsQueryParams? LastQueryParams { get; private set; }
        public CreateProductRequest? LastCreateRequest { get; private set; }
        public UpdateProductRequest? LastUpdateRequest { get; private set; }
        public int? LastUpdateProductId { get; private set; }
        public int? LastToggleProductId { get; private set; }

        public Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
        {
            LastGetByIdProductId = productId;
            return Task.FromResult(ProductById);
        }

        public Task<PagedResult<Product>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct = default)
        {
            LastQueryParams = queryParams;
            return Task.FromResult(PagedProducts);
        }

        public Task<(DbResultOption Result, int ProductId)> AddProductAsync(CreateProductRequest product, CancellationToken ct = default)
        {
            LastCreateRequest = product;
            return Task.FromResult(AddResult);
        }

        public Task<(DbResultOption Result, Product? Product)> UpdateProductAsync(UpdateProductRequest product, int productId, CancellationToken ct = default)
        {
            LastUpdateRequest = product;
            LastUpdateProductId = productId;
            return Task.FromResult(UpdateResult);
        }

        public Task<DbResultOption> ToggleProductAsync(int productId, CancellationToken ct = default)
        {
            LastToggleProductId = productId;
            return Task.FromResult(ToggleResult);
        }
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public Category? CategoryById { get; set; }
        public int? LastGetByIdCategoryId { get; private set; }

        public Task<Category?> GetByIdAsync(int categoryId, CancellationToken ct = default)
        {
            LastGetByIdCategoryId = categoryId;
            return Task.FromResult(CategoryById);
        }

        // --- Unused members in these tests ---
        public Task<PagedResult<Category>> GetAllCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<Category?> GetCategoryGraphByIdAsync(int id, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<Category>> GetRootsAsync(bool includeInactive = false, bool featuredOnly = false, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<Category>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(DbResultOption Result, int CategoryId)> AddCategoryAsync(CreateCategoryRequest category, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<DbResultOption> UpdateCategoryAsync(CreateCategoryRequest category, int categoryId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<DbResultOption> ToggleCategoryAsync(int categoryId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<DbResultOption> AttachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<DbResultOption> DetachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class FakeImageAssetRepository : IImageAssetRepository
    {
        public List<ImageAsset> Images { get; set; } = [];

        public Task<ImageAsset?> GetByIdAsync(Guid imageId, CancellationToken ct = default)
            => Task.FromResult(Images.FirstOrDefault(i => i.Id == imageId));

        public Task<List<ImageAsset>> GetByTypeAndOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
            => Task.FromResult(Images.Where(i => i.Type == type && i.OwnerId == ownerId).ToList());

        public Task<List<ImageAsset>> GetByTypeAndOwnersIdsAsync(ImageAssetType type, List<int> ownerIds, CancellationToken ct = default)
            => Task.FromResult(Images.Where(i => i.Type == type && ownerIds.Contains(i.OwnerId)).ToList());

        public Task<ImageAsset?> GetPrimaryByTypeAndOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
            => Task.FromResult(Images.FirstOrDefault(i => i.Type == type && i.OwnerId == ownerId && i.IsPrimary));

        public Task<ImageAsset> AddAsync(ImageAsset image, CancellationToken ct = default)
            => Task.FromResult(image);

        public Task<bool> DeleteAsync(Guid imageId, CancellationToken ct = default)
            => Task.FromResult(true);

        public Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken ct = default)
            => Task.FromResult(true);

        public Task<bool> UpdateDisplayOrderAsync(ImageAssetType type, int ownerId, IList<Guid> orderedImageIds, CancellationToken ct = default)
            => Task.FromResult(true);

        public Task<int> GetCountByOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
            => Task.FromResult(Images.Count(i => i.Type == type && i.OwnerId == ownerId));
    }

    private sealed class FakeEntityTranslationRepository : IEntityTranslationRepository
    {
        public EntityTranslation? SingleTranslation { get; set; }
        public List<EntityTranslation> BulkTranslations { get; set; } = [];
        public bool GetTranslationCalled { get; private set; }
        public bool GetTranslationsCalled { get; private set; }

        public Task<EntityTranslation?> GetTranslationAsync(
            TranslatableEntityType entityType, int entityId,
            string languageCode, CancellationToken ct = default)
        {
            GetTranslationCalled = true;
            return Task.FromResult(SingleTranslation);
        }

        public Task<IReadOnlyList<EntityTranslation>> GetTranslationsAsync(
            TranslatableEntityType entityType, IEnumerable<int> entityIds,
            string languageCode, CancellationToken ct = default)
        {
            GetTranslationsCalled = true;
            return Task.FromResult<IReadOnlyList<EntityTranslation>>(BulkTranslations);
        }
    }
}
