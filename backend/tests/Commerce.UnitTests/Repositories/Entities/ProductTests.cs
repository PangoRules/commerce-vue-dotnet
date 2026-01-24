using Commerce.Repositories.Entities;
using Commerce.Shared.Requests;

namespace Commerce.UnitTests.Repositories.Entities;

public class ProductTests
{
    [Fact]
    public void ToggleProduct_WhenInactive_SetsActive()
    {
        // Arrange
        var product = new Product
        {
            IsActive = false
        };

        // Act
        product.ToggleProduct();

        // Assert
        Assert.True(product.IsActive);
    }

    [Fact]
    public void ToggleProduct_WhenActive_SetsInactive()
    {
        // Arrange
        var product = new Product
        {
            IsActive = true
        };

        // Act
        product.ToggleProduct();

        // Assert
        Assert.False(product.IsActive);
    }

    [Fact]
    public void UpdateProduct_MapsAllFieldsFromRequest()
    {
        // Arrange
        var product = new Product
        {
            Id = 123,
            CategoryId = 1,
            Name = "Old Name",
            Description = "Old Desc",
            Price = 1.23m,
            StockQuantity = 1,
            IsActive = true
        };

        var request = new UpdateProductRequest
        {
            CategoryId = 99,
            Name = "New Name",
            Description = "New Desc",
            Price = 999.99m,
            StockQuantity = 420
        };

        // Act
        product.UpdateProduct(request);

        // Assert
        Assert.Equal(99, product.CategoryId);
        Assert.Equal("New Name", product.Name);
        Assert.Equal("New Desc", product.Description);
        Assert.Equal(999.99m, product.Price);
        Assert.Equal(420, product.StockQuantity);

        // Sanity: Update should not touch these
        Assert.Equal(123, product.Id);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void FromCreateRequest_CreatesEntityWithDefaultsAndCopiesFields()
    {
        // Arrange
        var before = DateTime.UtcNow;

        var request = new CreateProductRequest
        {
            CategoryId = 5,
            Name = "Coffee Maker",
            Description = "12-cup programmable coffee maker",
            Price = 79.99m,
            StockQuantity = 45
        };

        // Act
        var product = Product.FromCreateRequest(request);

        var after = DateTime.UtcNow;

        // Assert - copied fields
        Assert.Equal(5, product.CategoryId);
        Assert.Equal("Coffee Maker", product.Name);
        Assert.Equal("12-cup programmable coffee maker", product.Description);
        Assert.Equal(79.99m, product.Price);
        Assert.Equal(45, product.StockQuantity);
        // Assert - defaults
        Assert.True(product.IsActive);

        // Assert - CreatedAt set "around now"
        Assert.True(product.CreatedAt >= before && product.CreatedAt <= after);

        // Assert - Id should be default (EF sets later)
        Assert.Equal(0, product.Id);
    }

    [Fact]
    public void FromCreateRequest_AllowsNullDescription()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            CategoryId = 2,
            Name = "Clean Code",
            Description = null,
            Price = 34.99m,
            StockQuantity = 100
        };

        // Act
        var product = Product.FromCreateRequest(request);

        // Assert
        Assert.Null(product.Description);
    }
}
