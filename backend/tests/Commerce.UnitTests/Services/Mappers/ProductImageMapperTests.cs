using Commerce.Repositories.Entities;
using Commerce.Services.Mappers;

namespace Commerce.UnitTests.Services.Mappers;

public class ProductImageMapperTests
{
    [Fact]
    public void ToResponse_MapsAllFields_Correctly()
    {
        var id = Guid.NewGuid();
        var uploadedAt = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var image = new ProductImage
        {
            Id = id,
            ProductId = 42,
            ObjectKey = "products/42/image-123.webp",
            FileName = "my-product-photo.webp",
            ContentType = "image/webp",
            SizeBytes = 102400,
            DisplayOrder = 2,
            IsPrimary = true,
            UploadedAt = uploadedAt
        };

        var result = ProductImageMapper.ToResponse(image);

        Assert.Equal(id, result.Id);
        Assert.Equal(42, result.ProductId);
        Assert.Equal("my-product-photo.webp", result.FileName);
        Assert.Equal("image/webp", result.ContentType);
        Assert.Equal(102400, result.SizeBytes);
        Assert.Equal(2, result.DisplayOrder);
        Assert.True(result.IsPrimary);
        Assert.Equal(uploadedAt, result.UploadedAt);
        Assert.Equal($"/api/productimage/{id}", result.Url);
    }

    [Fact]
    public void ToResponse_UsesCustomBaseUrl_WhenProvided()
    {
        var id = Guid.NewGuid();
        var image = new ProductImage
        {
            Id = id,
            ProductId = 1,
            ObjectKey = "key",
            FileName = "file.jpg",
            ContentType = "image/jpeg",
            SizeBytes = 1000,
            DisplayOrder = 0,
            IsPrimary = false,
            UploadedAt = DateTime.UtcNow
        };

        var result = ProductImageMapper.ToResponse(image, baseUrl: "/custom/images");

        Assert.Equal($"/custom/images/{id}", result.Url);
    }

    [Fact]
    public void ToResponseList_MapsEmptyList_ReturnsEmptyList()
    {
        var images = new List<ProductImage>();

        var result = ProductImageMapper.ToResponseList(images);

        Assert.Empty(result);
    }

    [Fact]
    public void ToResponseList_MapsMultipleImages_PreservesOrder()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        var images = new List<ProductImage>
        {
            new()
            {
                Id = id1,
                ProductId = 1,
                ObjectKey = "key1",
                FileName = "first.jpg",
                ContentType = "image/jpeg",
                SizeBytes = 1000,
                DisplayOrder = 0,
                IsPrimary = true,
                UploadedAt = DateTime.UtcNow
            },
            new()
            {
                Id = id2,
                ProductId = 1,
                ObjectKey = "key2",
                FileName = "second.jpg",
                ContentType = "image/jpeg",
                SizeBytes = 2000,
                DisplayOrder = 1,
                IsPrimary = false,
                UploadedAt = DateTime.UtcNow
            },
            new()
            {
                Id = id3,
                ProductId = 1,
                ObjectKey = "key3",
                FileName = "third.jpg",
                ContentType = "image/jpeg",
                SizeBytes = 3000,
                DisplayOrder = 2,
                IsPrimary = false,
                UploadedAt = DateTime.UtcNow
            }
        };

        var result = ProductImageMapper.ToResponseList(images);

        Assert.Equal(3, result.Count);
        Assert.Equal(id1, result[0].Id);
        Assert.Equal(id2, result[1].Id);
        Assert.Equal(id3, result[2].Id);
        Assert.Equal("first.jpg", result[0].FileName);
        Assert.Equal("second.jpg", result[1].FileName);
        Assert.Equal("third.jpg", result[2].FileName);
    }

    [Fact]
    public void ToResponseList_UsesCustomBaseUrl_ForAllItems()
    {
        var images = new List<ProductImage>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = 1,
                ObjectKey = "key1",
                FileName = "a.jpg",
                ContentType = "image/jpeg",
                SizeBytes = 1000,
                DisplayOrder = 0,
                IsPrimary = false,
                UploadedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = 1,
                ObjectKey = "key2",
                FileName = "b.jpg",
                ContentType = "image/jpeg",
                SizeBytes = 1000,
                DisplayOrder = 1,
                IsPrimary = false,
                UploadedAt = DateTime.UtcNow
            }
        };

        var result = ProductImageMapper.ToResponseList(images, baseUrl: "/v2/images");

        Assert.All(result, r => Assert.StartsWith("/v2/images/", r.Url));
    }
}
