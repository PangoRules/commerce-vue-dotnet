using Commerce.Repositories.Entities;
using Commerce.Services.Mappers;
using Commerce.Shared.Enums;

namespace Commerce.UnitTests.Services.Mappers;

public class CategoryMapperTests
{
    [Fact]
    public void ToResponse_MapsAllFieldsCorrectly()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic gadgets",
            IsFeatured = true
        };

        // Act
        var response = CategoryMapper.ToResponse(category);

        // Assert
        Assert.Equal(1, response.Id);
        Assert.Equal("Electronics", response.Name);
        Assert.Equal("Electronic gadgets", response.Description);
        Assert.True(response.IsFeatured);
    }

    [Fact]
    public void ToResponse_WithTranslation_UsesTranslatedNameAndDescription()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic gadgets",
            IsFeatured = true
        };

        var translation = new EntityTranslation
        {
            Id = 1,
            EntityType = TranslatableEntityType.Category,
            EntityId = 1,
            LanguageCode = "es",
            Name = "Electrónica",
            Description = "Dispositivos electrónicos"
        };

        // Act
        var response = CategoryMapper.ToResponse(category, translation: translation);

        // Assert
        Assert.Equal("Electrónica", response.Name);
        Assert.Equal("Dispositivos electrónicos", response.Description);
        Assert.True(response.IsFeatured);
    }

    [Fact]
    public void ToResponse_WithNullTranslation_UsesOriginalValues()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic gadgets",
            IsFeatured = false
        };

        // Act
        var response = CategoryMapper.ToResponse(category, translation: null);

        // Assert
        Assert.Equal("Electronics", response.Name);
        Assert.Equal("Electronic gadgets", response.Description);
    }

    [Fact]
    public void ToResponse_WithTranslationHavingNullDescription_FallsBackToOriginalDescription()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic gadgets",
            IsFeatured = false
        };

        var translation = new EntityTranslation
        {
            Id = 1,
            EntityType = TranslatableEntityType.Category,
            EntityId = 1,
            LanguageCode = "es",
            Name = "Electrónica",
            Description = null
        };

        // Act
        var response = CategoryMapper.ToResponse(category, translation: translation);

        // Assert
        Assert.Equal("Electrónica", response.Name);
        Assert.Equal("Electronic gadgets", response.Description);
    }
}
