using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;

/// <summary>
/// Repository interface for product image data operations.
/// </summary>
public interface IProductImageRepository
{
    /// <summary>
    /// Gets a product image by its ID.
    /// </summary>
    /// <param name="imageId">The unique identifier of the image.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The product image if found, otherwise null.</returns>
    Task<ProductImage?> GetByIdAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Gets all images for a specific product, ordered by display order.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of product images ordered by DisplayOrder.</returns>
    Task<List<ProductImage>> GetByProductIdAsync(int productId, CancellationToken ct = default);

    /// <summary>
    /// Gets the primary image for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The primary image if one exists, otherwise null.</returns>
    Task<ProductImage?> GetPrimaryByProductIdAsync(int productId, CancellationToken ct = default);

    /// <summary>
    /// Adds a new product image.
    /// </summary>
    /// <param name="image">The product image entity to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The added product image.</returns>
    Task<ProductImage> AddAsync(ProductImage image, CancellationToken ct = default);

    /// <summary>
    /// Deletes a product image by its ID.
    /// </summary>
    /// <param name="imageId">The unique identifier of the image to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the image was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Sets the specified image as the primary image for its product.
    /// Clears the primary flag from any other images for the same product.
    /// </summary>
    /// <param name="imageId">The ID of the image to set as primary.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if successful, false if image not found.</returns>
    Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken ct = default);

    /// <summary>
    /// Updates the display order of images for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="orderedImageIds">Image IDs in the desired order.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if successful, false if any image not found.</returns>
    Task<bool> UpdateDisplayOrderAsync(int productId, IList<Guid> orderedImageIds, CancellationToken ct = default);

    /// <summary>
    /// Gets the count of images for a specific product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of images for the product.</returns>
    Task<int> GetCountByProductIdAsync(int productId, CancellationToken ct = default);
}

[ExcludeFromCodeCoverage]
public class ProductImageRepository(CommerceDbContext context) : IProductImageRepository
{
    /// <inheritdoc/>
    public Task<ProductImage?> GetByIdAsync(Guid imageId, CancellationToken ct = default)
    {
        return context.ProductImages
            .AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.Id == imageId, ct);
    }

    /// <inheritdoc/>
    public Task<List<ProductImage>> GetByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return context.ProductImages
            .AsNoTracking()
            .Where(pi => pi.ProductId == productId)
            .OrderBy(pi => pi.DisplayOrder)
            .ThenBy(pi => pi.UploadedAt)
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public Task<ProductImage?> GetPrimaryByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return context.ProductImages
            .AsNoTracking()
            .Where(pi => pi.ProductId == productId && pi.IsPrimary)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<ProductImage> AddAsync(ProductImage image, CancellationToken ct = default)
    {
        await context.ProductImages.AddAsync(image, ct);
        await context.SaveChangesAsync(ct);
        return image;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == imageId, ct);
        if (image is null) return false;

        context.ProductImages.Remove(image);
        await context.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken ct = default)
    {
        var image = await context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == imageId, ct);
        if (image is null) return false;

        // Clear primary flag from other images of the same product
        var otherImages = await context.ProductImages
            .Where(pi => pi.ProductId == image.ProductId && pi.IsPrimary && pi.Id != imageId)
            .ToListAsync(ct);

        foreach (var other in otherImages)
        {
            other.IsPrimary = false;
        }

        // Set the specified image as primary
        image.IsPrimary = true;

        await context.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateDisplayOrderAsync(int productId, IList<Guid> orderedImageIds, CancellationToken ct = default)
    {
        var images = await context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync(ct);

        // Verify all IDs belong to this product
        var imageDict = images.ToDictionary(pi => pi.Id);
        if (!orderedImageIds.All(id => imageDict.ContainsKey(id)))
            return false;

        // Update display order based on position in the list
        for (int i = 0; i < orderedImageIds.Count; i++)
        {
            if (imageDict.TryGetValue(orderedImageIds[i], out var image))
            {
                image.DisplayOrder = i;
            }
        }

        await context.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc/>
    public Task<int> GetCountByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return context.ProductImages
            .CountAsync(pi => pi.ProductId == productId, ct);
    }
}
