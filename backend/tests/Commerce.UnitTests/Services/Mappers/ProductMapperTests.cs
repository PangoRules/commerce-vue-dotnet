using Commerce.Repositories.Entities;
using Commerce.Services.Mappers;
using Commerce.Shared.Enums;
using Commerce.Shared.Responses;

namespace Commerce.UnitTests.Services.Mappers;

public class ProductMapperTests
{
    [Fact]
    public void ToResponse_MapsAllFieldsCorrectly()
    {
        // Arrange
        var product = new Product
        {
            Id = 42,
            CategoryId = 7,
            Name = "Air Fryer",
            Description = "Oil-free air fryer",
            Price = 129.99m,
            StockQuantity = 40,
            IsActive = true
        };

        // Act
        ProductResponse response = ProductMapper.ToResponse(product);

        // Assert
        Assert.Equal(42, response.Id);
        Assert.Equal(7, response.CategoryId);
        Assert.Equal("Air Fryer", response.Name);
        Assert.Equal("Oil-free air fryer", response.Description);
        Assert.Equal(129.99m, response.Price);
        Assert.Equal(40, response.StockQuantity);
        Assert.True(response.IsActive);
    }

    [Fact]
    public void ToResponse_AllowsNullDescription()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            CategoryId = 2,
            Name = "Book",
            Description = null,
            Price = 19.99m,
            StockQuantity = 100,
            IsActive = false
        };

        // Act
        var response = ProductMapper.ToResponse(product);

        // Assert
        Assert.Null(response.Description);
        Assert.False(response.IsActive);
    }

    [Fact]
    public void ToResponse_WithTranslation_UsesTranslatedNameAndDescription()
    {
        // Arrange
        var product = new Product
        {
            Id = 42,
            CategoryId = 7,
            Name = "Air Fryer",
            Description = "Oil-free air fryer",
            Price = 129.99m,
            StockQuantity = 40,
            IsActive = true
        };

        var translation = new EntityTranslation
        {
            Id = 1,
            EntityType = TranslatableEntityType.Product,
            EntityId = 42,
            LanguageCode = "es",
            Name = "Freidora de Aire",
            Description = "Freidora de aire sin aceite"
        };

        // Act
        var response = ProductMapper.ToResponse(product, translation: translation);

        // Assert
        Assert.Equal("Freidora de Aire", response.Name);
        Assert.Equal("Freidora de aire sin aceite", response.Description);
        Assert.Equal(129.99m, response.Price);
    }

    [Fact]
    public void ToResponse_WithNullTranslation_UsesOriginalValues()
    {
        // Arrange
        var product = new Product
        {
            Id = 42,
            CategoryId = 7,
            Name = "Air Fryer",
            Description = "Oil-free air fryer",
            Price = 129.99m,
            StockQuantity = 40,
            IsActive = true
        };

        // Act
        var response = ProductMapper.ToResponse(product, translation: null);

        // Assert
        Assert.Equal("Air Fryer", response.Name);
        Assert.Equal("Oil-free air fryer", response.Description);
    }

    [Fact]
    public void ToResponse_WithTranslationHavingNullDescription_FallsBackToOriginalDescription()
    {
        // Arrange
        var product = new Product
        {
            Id = 42,
            CategoryId = 7,
            Name = "Air Fryer",
            Description = "Oil-free air fryer",
            Price = 129.99m,
            StockQuantity = 40,
            IsActive = true
        };

        var translation = new EntityTranslation
        {
            Id = 1,
            EntityType = TranslatableEntityType.Product,
            EntityId = 42,
            LanguageCode = "es",
            Name = "Freidora de Aire",
            Description = null
        };

        // Act
        var response = ProductMapper.ToResponse(product, translation: translation);

        // Assert
        Assert.Equal("Freidora de Aire", response.Name);
        Assert.Equal("Oil-free air fryer", response.Description);
    }
}
