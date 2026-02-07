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
/// Controller for generic image asset operations.
/// </summary>
[ApiController]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class ImageAssetController(
    IImageAssetService imageService,
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
    /// Tries to parse a route string to an <see cref="ImageAssetType"/>.
    /// </summary>
    private static bool TryParseOwnerType(string ownerType, out ImageAssetType type)
    {
        return ownerType.ToLowerInvariant() switch
        {
            "product" or "products" => SetOut(ImageAssetType.Product, out type),
            "category" or "categories" => SetOut(ImageAssetType.Category, out type),
            _ => SetOut(default, out type, false)
        };

        static bool SetOut(ImageAssetType value, out ImageAssetType result, bool success = true)
        {
            result = value;
            return success;
        }
    }

    /// <summary>
    /// Gets an image by ID (proxy endpoint - streams image from storage).
    /// </summary>
    [HttpGet("api/imageasset/{id:guid}")]
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
    [HttpGet("api/imageasset/{id:guid}/metadata")]
    [ProducesResponseType(typeof(ImageAssetResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetImageMetadata(Guid id, CancellationToken ct)
    {
        var image = await imageService.GetImageAssetByIdAsync(id, ct);
        return image is not null ? Ok(image) : NotFound();
    }

    /// <summary>
    /// Gets all images for an owner.
    /// </summary>
    [HttpGet("api/{ownerType}/{ownerId:int}/images")]
    [ProducesResponseType(typeof(List<ImageAssetResponse>), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetOwnerImages(string ownerType, int ownerId, CancellationToken ct)
    {
        if (!TryParseOwnerType(ownerType, out var type))
            return BadRequest($"Invalid owner type '{ownerType}'. Supported types: product, category.");

        var images = await imageService.GetImagesByOwnerAsync(type, ownerId, ct);
        return images.Count > 0 ? Ok(images) : NoContent();
    }

    /// <summary>
    /// Uploads a new image for an owner.
    /// </summary>
    [HttpPost("api/{ownerType}/{ownerId:int}/images")]
    [ProducesResponseType(typeof(ImageAssetResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> UploadImage(string ownerType, int ownerId, IFormFile file, CancellationToken ct)
    {
        if (!TryParseOwnerType(ownerType, out var type))
            return BadRequest($"Invalid owner type '{ownerType}'. Supported types: product, category.");

        // Validate file
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.Length > MaxFileSizeBytes)
            return BadRequest($"File size exceeds maximum of {MaxFileSizeBytes / 1024 / 1024} MB.");

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedContentTypes)}");

        // Generate object key: {ownerType}/{ownerId}/{guid}-{filename}
        var sanitizedFileName = Path.GetFileName(file.FileName);
        var folderName = type == ImageAssetType.Product ? "products" : "categories";
        var objectKey = $"{folderName}/{ownerId}/{Guid.NewGuid()}-{sanitizedFileName}";

        // Upload to storage
        await using var stream = file.OpenReadStream();
        await storageService.UploadAsync(objectKey, stream, file.ContentType, ct);

        // Create metadata record
        var (result, image) = await imageService.CreateImageAsync(
            type,
            ownerId,
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
    [HttpDelete("api/imageasset/{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteImage(Guid id, CancellationToken ct)
    {
        var (result, objectKey) = await imageService.DeleteImageAsync(id, ct);

        if (result == DbResultOption.Success && objectKey is not null)
        {
            await storageService.DeleteAsync(objectKey, ct);
        }

        return this.ToActionResult(result, NoContent);
    }

    /// <summary>
    /// Sets an image as the primary image for its owner.
    /// </summary>
    [HttpPut("api/imageasset/{id:guid}/primary")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetPrimary(Guid id, CancellationToken ct)
    {
        var result = await imageService.SetPrimaryAsync(id, ct);
        return this.ToActionResult(result, NoContent);
    }

    /// <summary>
    /// Reorders images for an owner.
    /// </summary>
    [HttpPut("api/{ownerType}/{ownerId:int}/images/reorder")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ReorderImages(string ownerType, int ownerId, [FromBody] ReorderImagesRequest request, CancellationToken ct)
    {
        if (!TryParseOwnerType(ownerType, out var type))
            return BadRequest($"Invalid owner type '{ownerType}'. Supported types: product, category.");

        if (request.ImageIds is null || request.ImageIds.Count == 0)
            return BadRequest("Image IDs are required.");

        var result = await imageService.ReorderImagesAsync(type, ownerId, request.ImageIds, ct);
        return this.ToActionResult(result, NoContent);
    }
}
