using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Services.Mappers;
using Commerce.Shared.Enums;
using Commerce.Shared.Responses;

namespace Commerce.Services;

/// <summary>
/// Service interface for image asset business logic.
/// </summary>
public interface IImageAssetService
{
    /// <summary>
    /// Gets a single image asset by ID.
    /// </summary>
    Task<ImageAssetResponse?> GetImageAssetByIdAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Gets all images for an owner of a given type.
    /// </summary>
    Task<List<ImageAssetResponse>> GetImagesByOwnerAsync(ImageAssetType type, int ownerId, CancellationToken ct = default);

    /// <summary>
    /// Gets the object key for an image (used by controller for storage operations).
    /// </summary>
    Task<string?> GetObjectKeyAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Creates a new image asset metadata record.
    /// Call this after successfully uploading the file to storage.
    /// </summary>
    Task<(DbResultOption Result, ImageAssetResponse? Image)> CreateImageAsync(
        ImageAssetType type,
        int ownerId,
        string objectKey,
        string fileName,
        string contentType,
        long sizeBytes,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes an image metadata record.
    /// Returns the object key so the caller can delete from storage.
    /// </summary>
    Task<(DbResultOption Result, string? ObjectKey)> DeleteImageAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Sets an image as the primary image for its owner.
    /// </summary>
    Task<DbResultOption> SetPrimaryAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Reorders images for an owner.
    /// </summary>
    Task<DbResultOption> ReorderImagesAsync(ImageAssetType type, int ownerId, IList<Guid> orderedImageIds, CancellationToken ct = default);

    /// <summary>
    /// Gets the count of images for an owner (for validation/limits).
    /// </summary>
    Task<int> GetImageCountAsync(ImageAssetType type, int ownerId, CancellationToken ct = default);
}

/// <summary>
/// Implementation of image asset business logic.
/// </summary>
public class ImageAssetService(IImageAssetRepository repo) : IImageAssetService
{
    private const int MaxImagesPerOwner = 10;

    /// <inheritdoc/>
    public async Task<ImageAssetResponse?> GetImageAssetByIdAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await repo.GetByIdAsync(imageId, ct);
        return image is null ? null : ImageAssetMapper.ToResponse(image);
    }

    /// <inheritdoc/>
    public async Task<List<ImageAssetResponse>> GetImagesByOwnerAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
    {
        var images = await repo.GetByTypeAndOwnerIdAsync(type, ownerId, ct);
        return ImageAssetMapper.ToResponseList(images);
    }

    /// <inheritdoc/>
    public async Task<string?> GetObjectKeyAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await repo.GetByIdAsync(imageId, ct);
        return image?.ObjectKey;
    }

    /// <inheritdoc/>
    public async Task<(DbResultOption Result, ImageAssetResponse? Image)> CreateImageAsync(
        ImageAssetType type,
        int ownerId,
        string objectKey,
        string fileName,
        string contentType,
        long sizeBytes,
        CancellationToken ct = default)
    {
        // Check image limit
        var currentCount = await repo.GetCountByOwnerIdAsync(type, ownerId, ct);
        if (currentCount >= MaxImagesPerOwner)
            return (DbResultOption.Invalid, null);

        // Determine display order (add at end)
        var displayOrder = currentCount;

        // First image becomes primary automatically
        var isPrimary = currentCount == 0;

        var entity = ImageAsset.Create(
            type,
            ownerId,
            objectKey,
            fileName,
            contentType,
            sizeBytes,
            displayOrder,
            isPrimary);

        var created = await repo.AddAsync(entity, ct);
        return (DbResultOption.Success, ImageAssetMapper.ToResponse(created));
    }

    /// <inheritdoc/>
    public async Task<(DbResultOption Result, string? ObjectKey)> DeleteImageAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await repo.GetByIdAsync(imageId, ct);
        if (image is null)
            return (DbResultOption.NotFound, null);

        var objectKey = image.ObjectKey;
        var wasPrimary = image.IsPrimary;
        var type = image.Type;
        var ownerId = image.OwnerId;

        var deleted = await repo.DeleteAsync(imageId, ct);
        if (!deleted)
            return (DbResultOption.Error, null);

        // If we deleted the primary image, promote the first remaining image
        if (wasPrimary)
        {
            var remaining = await repo.GetByTypeAndOwnerIdAsync(type, ownerId, ct);
            if (remaining.Count > 0)
            {
                await repo.SetPrimaryAsync(remaining[0].Id, ct);
            }
        }

        return (DbResultOption.Success, objectKey);
    }

    /// <inheritdoc/>
    public async Task<DbResultOption> SetPrimaryAsync(Guid imageId, CancellationToken ct = default)
    {
        var success = await repo.SetPrimaryAsync(imageId, ct);
        return success ? DbResultOption.Success : DbResultOption.NotFound;
    }

    /// <inheritdoc/>
    public async Task<DbResultOption> ReorderImagesAsync(ImageAssetType type, int ownerId, IList<Guid> orderedImageIds, CancellationToken ct = default)
    {
        var success = await repo.UpdateDisplayOrderAsync(type, ownerId, orderedImageIds, ct);
        return success ? DbResultOption.Success : DbResultOption.Invalid;
    }

    /// <inheritdoc/>
    public Task<int> GetImageCountAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
    {
        return repo.GetCountByOwnerIdAsync(type, ownerId, ct);
    }
}
