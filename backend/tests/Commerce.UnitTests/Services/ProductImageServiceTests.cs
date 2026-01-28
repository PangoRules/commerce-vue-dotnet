using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Services;
using Commerce.Shared.Enums;

namespace Commerce.UnitTests.Services;

public class ProductImageServiceTests
{
    [Fact]
    public async Task GetImagesForProductAsync_ReturnsEmptyList_WhenNoImages()
    {
        var imageRepo = new FakeProductImageRepository { Images = [] };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetImagesForProductAsync(productId: 1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetImagesForProductAsync_ReturnsMappedImages_OrderedByDisplayOrder()
    {
        var imageRepo = new FakeProductImageRepository
        {
            Images =
            [
                CreateImage(Guid.NewGuid(), 1, "key1", "a.jpg", 0),
                CreateImage(Guid.NewGuid(), 1, "key2", "b.jpg", 1)
            ]
        };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetImagesForProductAsync(productId: 1);

        Assert.Equal(2, result.Count);
        Assert.Equal("a.jpg", result[0].FileName);
        Assert.Equal("b.jpg", result[1].FileName);
    }

    [Fact]
    public async Task GetImageByIdAsync_ReturnsNull_WhenNotFound()
    {
        var imageRepo = new FakeProductImageRepository { ImageById = null };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetImageByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetImageByIdAsync_ReturnsMappedImage_WhenFound()
    {
        var id = Guid.NewGuid();
        var imageRepo = new FakeProductImageRepository
        {
            ImageById = CreateImage(id, 1, "products/1/img.webp", "img.webp", 0)
        };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetImageByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.Equal("img.webp", result.FileName);
        Assert.Equal("/api/productimage/" + id, result.Url);
    }

    [Fact]
    public async Task GetObjectKeyAsync_ReturnsNull_WhenImageNotFound()
    {
        var imageRepo = new FakeProductImageRepository { ImageById = null };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetObjectKeyAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetObjectKeyAsync_ReturnsObjectKey_WhenFound()
    {
        var imageRepo = new FakeProductImageRepository
        {
            ImageById = CreateImage(Guid.NewGuid(), 1, "products/1/test.webp", "test.webp", 0)
        };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetObjectKeyAsync(Guid.NewGuid());

        Assert.Equal("products/1/test.webp", result);
    }

    [Fact]
    public async Task CreateImageAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var imageRepo = new FakeProductImageRepository();
        var productRepo = new FakeProductRepository { ProductById = null };

        var sut = new ProductImageService(imageRepo, productRepo);

        var (result, image) = await sut.CreateImageAsync(
            productId: 999,
            objectKey: "test/key",
            fileName: "test.jpg",
            contentType: "image/jpeg",
            sizeBytes: 1024);

        Assert.Equal(DbResultOption.NotFound, result);
        Assert.Null(image);
    }

    [Fact]
    public async Task CreateImageAsync_ReturnsInvalid_WhenMaxImagesReached()
    {
        var imageRepo = new FakeProductImageRepository { ImageCount = 10 };
        var productRepo = new FakeProductRepository
        {
            ProductById = new Product { Id = 1, Name = "Test", CategoryId = 1 }
        };

        var sut = new ProductImageService(imageRepo, productRepo);

        var (result, image) = await sut.CreateImageAsync(
            productId: 1,
            objectKey: "test/key",
            fileName: "test.jpg",
            contentType: "image/jpeg",
            sizeBytes: 1024);

        Assert.Equal(DbResultOption.Invalid, result);
        Assert.Null(image);
    }

    [Fact]
    public async Task CreateImageAsync_CreatesImage_WithCorrectDisplayOrder()
    {
        var imageRepo = new FakeProductImageRepository { ImageCount = 3 };
        var productRepo = new FakeProductRepository
        {
            ProductById = new Product { Id = 1, Name = "Test", CategoryId = 1 }
        };

        var sut = new ProductImageService(imageRepo, productRepo);

        var (result, image) = await sut.CreateImageAsync(
            productId: 1,
            objectKey: "test/key",
            fileName: "test.jpg",
            contentType: "image/jpeg",
            sizeBytes: 1024);

        Assert.Equal(DbResultOption.Success, result);
        Assert.NotNull(image);
        Assert.Equal(3, image!.DisplayOrder); // Should be at position 3 (0-indexed)
        Assert.False(image.IsPrimary); // Not primary since there are existing images
    }

    [Fact]
    public async Task CreateImageAsync_SetsFirstImageAsPrimary()
    {
        var imageRepo = new FakeProductImageRepository { ImageCount = 0 };
        var productRepo = new FakeProductRepository
        {
            ProductById = new Product { Id = 1, Name = "Test", CategoryId = 1 }
        };

        var sut = new ProductImageService(imageRepo, productRepo);

        var (result, image) = await sut.CreateImageAsync(
            productId: 1,
            objectKey: "test/key",
            fileName: "first.jpg",
            contentType: "image/jpeg",
            sizeBytes: 1024);

        Assert.Equal(DbResultOption.Success, result);
        Assert.NotNull(image);
        Assert.True(image!.IsPrimary);
        Assert.Equal(0, image.DisplayOrder);
    }

    [Fact]
    public async Task DeleteImageAsync_ReturnsNotFound_WhenImageDoesNotExist()
    {
        var imageRepo = new FakeProductImageRepository { ImageById = null };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var (result, objectKey) = await sut.DeleteImageAsync(Guid.NewGuid());

        Assert.Equal(DbResultOption.NotFound, result);
        Assert.Null(objectKey);
    }

    [Fact]
    public async Task DeleteImageAsync_ReturnsObjectKey_ForStorageCleanup()
    {
        var id = Guid.NewGuid();
        var imageRepo = new FakeProductImageRepository
        {
            ImageById = CreateImage(id, 1, "products/1/delete-me.webp", "delete-me.webp", 0, isPrimary: false),
            DeleteSuccess = true,
            Images = []
        };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var (result, objectKey) = await sut.DeleteImageAsync(id);

        Assert.Equal(DbResultOption.Success, result);
        Assert.Equal("products/1/delete-me.webp", objectKey);
    }

    [Fact]
    public async Task DeleteImageAsync_PromotesNextImage_WhenDeletingPrimary()
    {
        var primaryId = Guid.NewGuid();
        var nextId = Guid.NewGuid();

        var imageRepo = new FakeProductImageRepository
        {
            ImageById = CreateImage(primaryId, 1, "key1", "primary.jpg", 0, isPrimary: true),
            DeleteSuccess = true,
            Images = [CreateImage(nextId, 1, "key2", "next.jpg", 1, isPrimary: false)]
        };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        await sut.DeleteImageAsync(primaryId);

        Assert.Equal(nextId, imageRepo.LastSetPrimaryId);
    }

    [Fact]
    public async Task SetPrimaryAsync_ReturnsNotFound_WhenImageDoesNotExist()
    {
        var imageRepo = new FakeProductImageRepository { SetPrimarySuccess = false };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.SetPrimaryAsync(Guid.NewGuid());

        Assert.Equal(DbResultOption.NotFound, result);
    }

    [Fact]
    public async Task SetPrimaryAsync_ReturnsSuccess_WhenImageExists()
    {
        var imageRepo = new FakeProductImageRepository { SetPrimarySuccess = true };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.SetPrimaryAsync(Guid.NewGuid());

        Assert.Equal(DbResultOption.Success, result);
    }

    [Fact]
    public async Task ReorderImagesAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var imageRepo = new FakeProductImageRepository();
        var productRepo = new FakeProductRepository { ProductById = null };

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.ReorderImagesAsync(999, [Guid.NewGuid()]);

        Assert.Equal(DbResultOption.NotFound, result);
    }

    [Fact]
    public async Task ReorderImagesAsync_ReturnsInvalid_WhenImageIdsMismatch()
    {
        var imageRepo = new FakeProductImageRepository { UpdateDisplayOrderSuccess = false };
        var productRepo = new FakeProductRepository
        {
            ProductById = new Product { Id = 1, Name = "Test", CategoryId = 1 }
        };

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.ReorderImagesAsync(1, [Guid.NewGuid()]);

        Assert.Equal(DbResultOption.Invalid, result);
    }

    [Fact]
    public async Task ReorderImagesAsync_ReturnsSuccess_WhenValid()
    {
        var imageRepo = new FakeProductImageRepository { UpdateDisplayOrderSuccess = true };
        var productRepo = new FakeProductRepository
        {
            ProductById = new Product { Id = 1, Name = "Test", CategoryId = 1 }
        };

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.ReorderImagesAsync(1, [Guid.NewGuid(), Guid.NewGuid()]);

        Assert.Equal(DbResultOption.Success, result);
    }

    [Fact]
    public async Task GetImageCountAsync_ReturnsCount()
    {
        var imageRepo = new FakeProductImageRepository { ImageCount = 5 };
        var productRepo = new FakeProductRepository();

        var sut = new ProductImageService(imageRepo, productRepo);

        var result = await sut.GetImageCountAsync(1);

        Assert.Equal(5, result);
    }

    // ----------------------------
    // Helper
    // ----------------------------

    private static ProductImage CreateImage(
        Guid id, int productId, string objectKey, string fileName,
        int displayOrder, bool isPrimary = false)
    {
        return new ProductImage
        {
            Id = id,
            ProductId = productId,
            ObjectKey = objectKey,
            FileName = fileName,
            ContentType = "image/jpeg",
            SizeBytes = 1024,
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary,
            UploadedAt = DateTime.UtcNow
        };
    }

    // ----------------------------
    // Fakes
    // ----------------------------

    private sealed class FakeProductImageRepository : IProductImageRepository
    {
        public ProductImage? ImageById { get; set; }
        public List<ProductImage> Images { get; set; } = [];
        public int ImageCount { get; set; } = 0;
        public bool DeleteSuccess { get; set; } = true;
        public bool SetPrimarySuccess { get; set; } = true;
        public bool UpdateDisplayOrderSuccess { get; set; } = true;

        public Guid? LastSetPrimaryId { get; private set; }
        public ProductImage? LastAddedImage { get; private set; }

        public Task<ProductImage?> GetByIdAsync(Guid imageId, CancellationToken ct = default)
            => Task.FromResult(ImageById);

        public Task<List<ProductImage>> GetByProductIdAsync(int productId, CancellationToken ct = default)
            => Task.FromResult(Images);

        public Task<ProductImage?> GetPrimaryByProductIdAsync(int productId, CancellationToken ct = default)
            => Task.FromResult(Images.FirstOrDefault(i => i.IsPrimary));

        public Task<ProductImage> AddAsync(ProductImage image, CancellationToken ct = default)
        {
            LastAddedImage = image;
            return Task.FromResult(image);
        }

        public Task<bool> DeleteAsync(Guid imageId, CancellationToken ct = default)
            => Task.FromResult(DeleteSuccess);

        public Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken ct = default)
        {
            LastSetPrimaryId = imageId;
            return Task.FromResult(SetPrimarySuccess);
        }

        public Task<bool> UpdateDisplayOrderAsync(int productId, IList<Guid> orderedImageIds, CancellationToken ct = default)
            => Task.FromResult(UpdateDisplayOrderSuccess);

        public Task<int> GetCountByProductIdAsync(int productId, CancellationToken ct = default)
            => Task.FromResult(ImageCount);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public Product? ProductById { get; set; }

        public Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
            => Task.FromResult(ProductById);

        // Unused in these tests
        public Task<Commerce.Shared.Responses.PagedResult<Product>> GetAllProductsAsync(
            Commerce.Shared.Requests.GetProductsQueryParams queryParams, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(DbResultOption Result, int ProductId)> AddProductAsync(
            Commerce.Shared.Requests.CreateProductRequest product, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(DbResultOption Result, Product? Product)> UpdateProductAsync(
            Commerce.Shared.Requests.UpdateProductRequest product, int productId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<DbResultOption> ToggleProductAsync(int productId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
