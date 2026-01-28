using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

/// <summary>
/// Maps ProductImage entities to response DTOs.
/// </summary>
public static class ProductImageMapper
{
    /// <summary>
    /// Converts a ProductImage entity to a response DTO.
    /// </summary>
    /// <param name="image">The product image entity.</param>
    /// <param name="baseUrl">Base URL for the proxy endpoint (e.g., "/api/productimage").</param>
    /// <returns>The product image response DTO.</returns>
    public static ProductImageResponse ToResponse(ProductImage image, string baseUrl = "/api/productimage")
    {
        return new ProductImageResponse
        {
            Id = image.Id,
            ProductId = image.ProductId,
            FileName = image.FileName,
            ContentType = image.ContentType,
            SizeBytes = image.SizeBytes,
            DisplayOrder = image.DisplayOrder,
            IsPrimary = image.IsPrimary,
            UploadedAt = image.UploadedAt,
            Url = $"{baseUrl}/{image.Id}"
        };
    }

    /// <summary>
    /// Converts a list of ProductImage entities to response DTOs.
    /// </summary>
    public static List<ProductImageResponse> ToResponseList(IEnumerable<ProductImage> images, string baseUrl = "/api/productimage")
    {
        return images.Select(i => ToResponse(i, baseUrl)).ToList();
    }
}
