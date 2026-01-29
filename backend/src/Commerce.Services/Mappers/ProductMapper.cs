using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product) =>
        new()
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            Category = product.Category != null
                ? new CategoryResponse
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                }
                : null,
            Images = ProductImageMapper.ToResponseList(product.Images),
            PrimaryImageUrl = GetPrimaryImageUrl(product.Images)
        };

    private static string? GetPrimaryImageUrl(ICollection<ProductImage> images)
    {
        if (images == null || images.Count == 0) return null;

        // Find primary image, or fall back to first by DisplayOrder
        var primary = images.FirstOrDefault(i => i.IsPrimary)
                      ?? images.OrderBy(i => i.DisplayOrder).First();

        return $"/api/productimage/{primary.Id}";
    }
}
