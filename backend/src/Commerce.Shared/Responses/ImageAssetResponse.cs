using System;
using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Enums;

namespace Commerce.Shared.Responses;

[ExcludeFromCodeCoverage]
public class ImageAssetResponse
{
    /// <summary>
    /// Image id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Type of image asset. <see cref="ImageAssetType"/>
    /// </summary>
    public ImageAssetType Type { get; set; }

    /// <summary>
    /// ID of the product or category this image belongs to.
    /// </summary>
    public int OwnerId { get; set; }

    /// <summary>
    /// Original filename uploaded by the user.
    /// </summary>
    public string FileName { get; init; } = null!;

    /// <summary>
    /// MIME type of the image.
    /// </summary>
    public string ContentType { get; init; } = null!;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeBytes { get; init; }

    /// <summary>
    /// Display order (lower = first).
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Whether this is the primary/featured image.
    /// </summary>
    public bool IsPrimary { get; init; }

    /// <summary>
    /// When the image was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; init; }

    /// <summary>
    /// URL to access the image via the proxy endpoint.
    /// Example: "/api/productimage/{id}"
    /// </summary>
    public string Url { get; init; } = null!;
}
