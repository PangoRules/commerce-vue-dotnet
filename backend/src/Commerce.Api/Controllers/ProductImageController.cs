using System.Diagnostics.CodeAnalysis;
using Commerce.Api.Mappers;
using Commerce.Api.Storage;
using Commerce.Services;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

/// <summary>
/// Controller for product image operations.
/// </summary>
[ApiController]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class ProductImageController(
    IProductImageService imageService,
    IObjectStorageService storageService
) : ControllerBase
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    ];

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    /// <summary>
    /// Gets an image by ID (proxy endpoint - streams image from storage).
    /// </summary>
    /// <param name="id">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The image file.</returns>
    [HttpGet("api/productimage/{id:guid}")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetImage(Guid id, CancellationToken ct)
    {
        var objectKey = await imageService.GetObjectKeyAsync(id, ct);
        if (objectKey is null)
            return NotFound();

        var result = await storageService.GetObjectAsync(objectKey, ct);
        if (result is null)
            return NotFound();

        return File(result.Stream, result.ContentType);
    }

    /// <summary>
    /// Gets image metadata by ID.
    /// </summary>
    /// <param name="id">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("api/productimage/{id:guid}/metadata")]
    [ProducesResponseType(typeof(ProductImageResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetImageMetadata(Guid id, CancellationToken ct)
    {
        var image = await imageService.GetImageByIdAsync(id, ct);
        return image is not null ? Ok(image) : NotFound();
    }

    /// <summary>
    /// Gets all images for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("api/product/{productId:int}/images")]
    [ProducesResponseType(typeof(List<ProductImageResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetProductImages(int productId, CancellationToken ct)
    {
        var images = await imageService.GetImagesForProductAsync(productId, ct);
        return images.Count > 0 ? Ok(images) : NoContent();
    }

    /// <summary>
    /// Uploads a new image for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="file">The image file to upload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost("api/product/{productId:int}/images")]
    [ProducesResponseType(typeof(ProductImageResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> UploadImage(int productId, IFormFile file, CancellationToken ct)
    {
        // Validate file
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.Length > MaxFileSizeBytes)
            return BadRequest($"File size exceeds maximum of {MaxFileSizeBytes / 1024 / 1024} MB.");

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedContentTypes)}");

        // Generate object key: products/{productId}/{guid}-{filename}
        var sanitizedFileName = Path.GetFileName(file.FileName);
        var objectKey = $"products/{productId}/{Guid.NewGuid()}-{sanitizedFileName}";

        // Upload to storage
        await using var stream = file.OpenReadStream();
        await storageService.UploadAsync(objectKey, stream, file.ContentType, ct);

        // Create metadata record
        var (result, image) = await imageService.CreateImageAsync(
            productId,
            objectKey,
            sanitizedFileName,
            file.ContentType,
            file.Length,
            ct);

        if (result != DbResultOption.Success)
        {
            // Rollback: delete from storage if DB insert failed
            await storageService.DeleteAsync(objectKey, ct);
            return this.ToActionResult(result, () => Ok());
        }

        return CreatedAtAction(
            nameof(GetImageMetadata),
            new { id = image!.Id },
            image);
    }

    /// <summary>
    /// Deletes an image.
    /// </summary>
    /// <param name="id">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpDelete("api/productimage/{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteImage(Guid id, CancellationToken ct)
    {
        var (result, objectKey) = await imageService.DeleteImageAsync(id, ct);

        if (result == DbResultOption.Success && objectKey is not null)
        {
            // Delete from storage (fire-and-forget is ok, but we'll await for consistency)
            await storageService.DeleteAsync(objectKey, ct);
        }

        return this.ToActionResult(result, NoContent);
    }

    /// <summary>
    /// Sets an image as the primary image for its product.
    /// </summary>
    /// <param name="id">The image ID.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPut("api/productimage/{id:guid}/primary")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetPrimary(Guid id, CancellationToken ct)
    {
        var result = await imageService.SetPrimaryAsync(id, ct);
        return this.ToActionResult(result, NoContent);
    }

    /// <summary>
    /// Reorders images for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="request">The reorder request containing image IDs in desired order.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPut("api/product/{productId:int}/images/reorder")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ReorderImages(int productId, [FromBody] ReorderImagesRequest request, CancellationToken ct)
    {
        if (request.ImageIds is null || request.ImageIds.Count == 0)
            return BadRequest("Image IDs are required.");

        var result = await imageService.ReorderImagesAsync(productId, request.ImageIds, ct);
        return this.ToActionResult(result, NoContent);
    }
}
