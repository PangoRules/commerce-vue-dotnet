using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Services.Mappers;
using Commerce.Shared.Enums;
using Commerce.Shared.Responses;

namespace Commerce.Services;

/// <summary>
/// Service interface for product image business logic.
/// </summary>
public interface IProductImageService
{
    /// <summary>
    /// Gets all images for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of product image responses ordered by display order.</returns>
    Task<List<ProductImageResponse>> GetImagesForProductAsync(int productId, CancellationToken ct = default);

    /// <summary>
    /// Gets a single image by ID.
    /// </summary>
    /// <param name="imageId">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The image response, or null if not found.</returns>
    Task<ProductImageResponse?> GetImageByIdAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Gets the object key for an image (used by controller for storage operations).
    /// </summary>
    /// <param name="imageId">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The object key, or null if not found.</returns>
    Task<string?> GetObjectKeyAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Creates a new product image metadata record.
    /// Call this after successfully uploading the file to storage.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="objectKey">The storage object key.</param>
    /// <param name="fileName">Original filename.</param>
    /// <param name="contentType">MIME type.</param>
    /// <param name="sizeBytes">File size in bytes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result and the created image response.</returns>
    Task<(DbResultOption Result, ProductImageResponse? Image)> CreateImageAsync(
        int productId,
        string objectKey,
        string fileName,
        string contentType,
        long sizeBytes,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes an image metadata record.
    /// Returns the object key so the caller can delete from storage.
    /// </summary>
    /// <param name="imageId">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result and the object key (for storage deletion).</returns>
    Task<(DbResultOption Result, string? ObjectKey)> DeleteImageAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Sets an image as the primary image for its product.
    /// </summary>
    /// <param name="imageId">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> SetPrimaryAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Reorders images for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="orderedImageIds">Image IDs in desired order.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> ReorderImagesAsync(int productId, IList<Guid> orderedImageIds, CancellationToken ct = default);

    /// <summary>
    /// Gets the count of images for a product (for validation/limits).
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of images.</returns>
    Task<int> GetImageCountAsync(int productId, CancellationToken ct = default);
}

/// <summary>
/// Implementation of product image business logic.
/// </summary>
public class ProductImageService(
    IProductImageRepository imageRepo,
    IProductRepository productRepo
) : IProductImageService
{
    private const int MaxImagesPerProduct = 10;

    /// <inheritdoc/>
    public async Task<List<ProductImageResponse>> GetImagesForProductAsync(int productId, CancellationToken ct = default)
    {
        var images = await imageRepo.GetByProductIdAsync(productId, ct);
        return ProductImageMapper.ToResponseList(images);
    }

    /// <inheritdoc/>
    public async Task<ProductImageResponse?> GetImageByIdAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await imageRepo.GetByIdAsync(imageId, ct);
        return image is null ? null : ProductImageMapper.ToResponse(image);
    }

    /// <inheritdoc/>
    public async Task<string?> GetObjectKeyAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await imageRepo.GetByIdAsync(imageId, ct);
        return image?.ObjectKey;
    }

    /// <inheritdoc/>
    public async Task<(DbResultOption Result, ProductImageResponse? Image)> CreateImageAsync(
        int productId,
        string objectKey,
        string fileName,
        string contentType,
        long sizeBytes,
        CancellationToken ct = default)
    {
        // Verify product exists
        var product = await productRepo.GetProductByIdAsync(productId, ct);
        if (product is null)
            return (DbResultOption.NotFound, null);

        // Check image limit
        var currentCount = await imageRepo.GetCountByProductIdAsync(productId, ct);
        if (currentCount >= MaxImagesPerProduct)
            return (DbResultOption.Invalid, null);

        // Determine display order (add at end)
        var displayOrder = currentCount;

        // First image becomes primary automatically
        var isPrimary = currentCount == 0;

        var entity = ProductImage.Create(
            productId,
            objectKey,
            fileName,
            contentType,
            sizeBytes,
            displayOrder,
            isPrimary);

        var created = await imageRepo.AddAsync(entity, ct);
        return (DbResultOption.Success, ProductImageMapper.ToResponse(created));
    }

    /// <inheritdoc/>
    public async Task<(DbResultOption Result, string? ObjectKey)> DeleteImageAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await imageRepo.GetByIdAsync(imageId, ct);
        if (image is null)
            return (DbResultOption.NotFound, null);

        var objectKey = image.ObjectKey;
        var wasPrimary = image.IsPrimary;
        var productId = image.ProductId;

        var deleted = await imageRepo.DeleteAsync(imageId, ct);
        if (!deleted)
            return (DbResultOption.Error, null);

        // If we deleted the primary image, promote the first remaining image
        if (wasPrimary)
        {
            var remaining = await imageRepo.GetByProductIdAsync(productId, ct);
            if (remaining.Count > 0)
            {
                await imageRepo.SetPrimaryAsync(remaining[0].Id, ct);
            }
        }

        return (DbResultOption.Success, objectKey);
    }

    /// <inheritdoc/>
    public async Task<DbResultOption> SetPrimaryAsync(Guid imageId, CancellationToken ct = default)
    {
        var success = await imageRepo.SetPrimaryAsync(imageId, ct);
        return success ? DbResultOption.Success : DbResultOption.NotFound;
    }

    /// <inheritdoc/>
    public async Task<DbResultOption> ReorderImagesAsync(int productId, IList<Guid> orderedImageIds, CancellationToken ct = default)
    {
        // Verify product exists
        var product = await productRepo.GetProductByIdAsync(productId, ct);
        if (product is null)
            return DbResultOption.NotFound;

        var success = await imageRepo.UpdateDisplayOrderAsync(productId, orderedImageIds, ct);
        return success ? DbResultOption.Success : DbResultOption.Invalid;
    }

    /// <inheritdoc/>
    public Task<int> GetImageCountAsync(int productId, CancellationToken ct = default)
    {
        return imageRepo.GetCountByProductIdAsync(productId, ct);
    }
}
