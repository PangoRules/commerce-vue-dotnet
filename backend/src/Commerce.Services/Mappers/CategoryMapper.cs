using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(
        Category category,
        IEnumerable<ImageAsset>? imageAssets = null,
        EntityTranslation? translation = null) =>
        new()
        {
            Id = category.Id,
            Name = translation?.Name ?? category.Name,
            Description = translation?.Description ?? category.Description,
            IsFeatured = category.IsFeatured,
            Images = imageAssets != null
                ? ImageAssetMapper.ToResponseList([.. imageAssets])
                : [],
            PrimaryImageUrl = GetPrimaryImageUrl(imageAssets)
        };

    private static string? GetPrimaryImageUrl(IEnumerable<ImageAsset>? images)
    {
        if (images is null || !images.Any()) return null;

        var primary = images.FirstOrDefault(i => i.IsPrimary)
                      ?? images.OrderBy(i => i.DisplayOrder).First();

        return $"/api/imageasset/{primary.Id}";
    }
}
