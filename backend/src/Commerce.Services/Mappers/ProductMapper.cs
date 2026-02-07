using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product, IEnumerable<ImageAsset>? imageAssets = null) =>
        new()
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            SalePrice = product.SalePrice,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            Category = product.Category != null
                ? new CategoryResponse
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description,
                    IsFeatured = product.Category.IsFeatured
                }
                : null,
            Images = imageAssets != null
                ? ImageAssetMapper.ToResponseList([.. imageAssets])
                : [],
            PrimaryImageUrl = GetPrimaryImageUrl(imageAssets)
        };

    private static string? GetPrimaryImageUrl(IEnumerable<ImageAsset>? images)
    {
        if (images is null || !images.Any()) return null;

        // Find primary image, or fall back to first by DisplayOrder
        var primary = images.FirstOrDefault(i => i.IsPrimary)
                      ?? images.OrderBy(i => i.DisplayOrder).First();

        return $"/api/imageasset/{primary.Id}";
    }
}
