using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;

public interface IImageAssetRepository
{
    /// <summary>
    /// Gets an image by ID
    /// </summary>
    /// <param name="imageId">Image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The image if found</returns>
    Task<ImageAsset?> GetByIdAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Gets all images for an <see cref="ImageAssetType"/> and owner
    /// </summary>
    /// <param name="ownerId">The owner ID</param>
    /// <param name="type">The image type</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images</returns>
    Task<List<ImageAsset>> GetByTypeAndOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default);

    /// <summary>
    /// Gets all images for an <see cref="ImageAssetType"/> and owners
    /// </summary>
    /// <param name="type">The image type</param>
    /// <param name="ownerIds">The owner IDs</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images</returns>
    Task<List<ImageAsset>> GetByTypeAndOwnersIdsAsync(ImageAssetType type, List<int> ownerIds, CancellationToken ct = default);

    /// <summary>
    /// Gets the primary image for an <see cref="ImageAssetType"/> and owner
    /// </summary>
    /// <param name="type">The image type</param>
    /// <param name="ownerId">The owner ID</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The primary image if found</returns>
    Task<ImageAsset?> GetPrimaryByTypeAndOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default);

    /// <summary>
    /// Adds a new image
    /// </summary>
    /// <param name="image">The image entity to add</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The added image</returns>
    Task<ImageAsset> AddAsync(ImageAsset image, CancellationToken ct = default);

    /// <summary>
    /// Deletes an image
    /// </summary>
    /// <param name="imageId">The image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if deleted</returns>
    Task<bool> DeleteAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Sets the primary image for an <see cref="ImageAssetType"/> and owner
    /// </summary>
    /// <param name="imageId">The image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if set</returns>
    Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Updates the display order of images for an <see cref="ImageAssetType"/> and owner
    /// </summary>
    /// <param name="type">The image type</param>
    /// <param name="ownerId">The owner ID</param>
    /// <param name="orderedImageIds">The ordered image IDs</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if updated</returns>
    Task<bool> UpdateDisplayOrderAsync(ImageAssetType type, int ownerId, IList<Guid> orderedImageIds, CancellationToken ct = default);

    /// <summary>
    /// Gets the count of images for an <see cref="ImageAssetType"/> and owner
    /// </summary>
    /// <param name="type">The image type</param>
    /// <param name="ownerId">The owner ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The count</returns>
    Task<int> GetCountByOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default);
}

[ExcludeFromCodeCoverage]
public class ImageAssetRepository(CommerceDbContext context) : IImageAssetRepository
{
    public async Task<ImageAsset> AddAsync(ImageAsset image, CancellationToken ct = default)
    {
        await context.ImageAssets.AddAsync(image, ct);
        await context.SaveChangesAsync(ct);
        return image;
    }

    public async Task<bool> DeleteAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await context.ImageAssets.FirstOrDefaultAsync(pi => pi.Id == imageId, ct);
        if (image is null) return false;

        context.ImageAssets.Remove(image);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public Task<ImageAsset?> GetByIdAsync(Guid imageId, CancellationToken ct = default)
    {
        return context.ImageAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.Id == imageId, ct);
    }

    public Task<List<ImageAsset>> GetByTypeAndOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
    {
        return context.ImageAssets
            .AsNoTracking()
            .Where(pi => pi.Type == type && pi.OwnerId == ownerId)
            .OrderBy(pi => pi.DisplayOrder)
            .ThenBy(pi => pi.UploadedAt)
            .ToListAsync(ct);
    }

    public Task<List<ImageAsset>> GetByTypeAndOwnersIdsAsync(ImageAssetType type, List<int> ownerIds, CancellationToken ct = default)
    {
        return context.ImageAssets
            .AsNoTracking()
            .Where(pi => pi.Type == type && ownerIds.Contains(pi.OwnerId))
            .OrderBy(pi => pi.DisplayOrder)
            .ThenBy(pi => pi.UploadedAt)
            .ToListAsync(ct);
    }

    public Task<int> GetCountByOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
    {
        return context.ImageAssets
            .CountAsync(pi => pi.Type == type && pi.OwnerId == ownerId, ct);
    }

    public Task<ImageAsset?> GetPrimaryByTypeAndOwnerIdAsync(ImageAssetType type, int ownerId, CancellationToken ct = default)
    {
        return context.ImageAssets
            .AsNoTracking()
            .Where(pi => pi.Type == type && pi.OwnerId == ownerId && pi.IsPrimary)
            .FirstOrDefaultAsync(ct);
    }

    public Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = context.ImageAssets.FirstOrDefault(pi => pi.Id == imageId);
        if (image is null) return Task.FromResult(false);

        // Clear primary flag from other images of the same type and owner
        var otherImages = context.ImageAssets
            .Where(pi => pi.Type == image.Type && pi.OwnerId == image.OwnerId && pi.IsPrimary && pi.Id != imageId)
            .ToList();

        foreach (var other in otherImages)
        {
            other.IsPrimary = false;
        }

        // Set the specified image as primary
        image.IsPrimary = true;

        context.SaveChanges();
        return Task.FromResult(true);
    }

    public Task<bool> UpdateDisplayOrderAsync(ImageAssetType type, int ownerId, IList<Guid> orderedImageIds, CancellationToken ct = default)
    {
        var images = context.ImageAssets
            .Where(pi => pi.Type == type && pi.OwnerId == ownerId)
            .ToList();

        // Verify all IDs belong to this type and owner
        var imageDict = images.ToDictionary(pi => pi.Id);
        if (!orderedImageIds.All(id => imageDict.ContainsKey(id) && imageDict[id].Type == type && imageDict[id].OwnerId == ownerId))
            return Task.FromResult(false);

        // Update display order based on position in the list
        for (int i = 0; i < orderedImageIds.Count; i++)
        {
            if (imageDict.TryGetValue(orderedImageIds[i], out var image))
            {
                image.DisplayOrder = i;
            }
        }

        context.SaveChanges();
        return Task.FromResult(true);
    }
}
