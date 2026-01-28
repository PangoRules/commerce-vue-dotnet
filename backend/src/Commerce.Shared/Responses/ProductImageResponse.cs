using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Responses;

/// <summary>
/// Response DTO for a product image.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProductImageResponse
{
    /// <summary>
    /// Unique identifier for the image.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The product this image belongs to.
    /// </summary>
    public int ProductId { get; init; }

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
