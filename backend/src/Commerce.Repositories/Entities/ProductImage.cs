namespace Commerce.Repositories.Entities;

/// <summary>
/// Represents an image associated with a product, stored in object storage.
/// </summary>
public class ProductImage
{
    /// <summary>
    /// Unique identifier for the product image.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the associated product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The object key (path) in the storage bucket.
    /// Example: "products/123/a1b2c3d4-image.webp"
    /// </summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>
    /// Original filename uploaded by the user.
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// MIME type of the image (e.g., "image/webp", "image/jpeg").
    /// </summary>
    public string ContentType { get; set; } = null!;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Display order for sorting images. Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Indicates if this is the primary/featured image for the product.
    /// Only one image per product should be marked as primary.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Timestamp when the image was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Navigation property to the parent product.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Creates a new ProductImage for the specified product.
    /// </summary>
    public static ProductImage Create(
        int productId,
        string objectKey,
        string fileName,
        string contentType,
        long sizeBytes,
        int displayOrder = 0,
        bool isPrimary = false)
    {
        return new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ObjectKey = objectKey,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary,
            UploadedAt = DateTime.UtcNow
        };
    }
}
