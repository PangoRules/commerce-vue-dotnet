using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

public class ImageAssetMapper
{
    /// <summary>
    /// Converts a ImageAsset entity to a response DTO
    /// </summary>
    /// <param name="asset">The image asset entity</param>
    /// <param name="baseUrl">The base URL for the proxy endpoint</param>
    /// <returns>The image asset response DTO</returns>
    public static ImageAssetResponse ToResponse(ImageAsset asset, string baseUrl = "/api/imageasset") =>
        new()
        {
            Id = asset.Id,
            OwnerId = asset.OwnerId,
            Type = asset.Type,
            FileName = asset.FileName,
            ContentType = asset.ContentType,
            SizeBytes = asset.SizeBytes,
            DisplayOrder = asset.DisplayOrder,
            IsPrimary = asset.IsPrimary,
            UploadedAt = asset.UploadedAt,
            Url = $"{baseUrl}/{asset.Id}"
        };

    /// <summary>
    /// Converts a list of ImageAsset entities to a list of response DTOs
    /// </summary>
    /// <param name="assets">The list of image asset entities</param>
    /// <param name="baseUrl">The base URL for the proxy endpoint</param>
    /// <returns>List of image asset response DTOs</returns>
    public static List<ImageAssetResponse> ToResponseList(List<ImageAsset> assets, string baseUrl = "/api/imageasset") =>
        [.. assets.Select(a => ToResponse(a, baseUrl))];
}
