
using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Enums;

namespace Commerce.Repositories.Entities;

[ExcludeFromCodeCoverage]
public class ImageAsset
{
	/// <summary>
	/// Unique identifier for the image.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Type of the image (product or category, more can be added).
	/// </summary>
	public ImageAssetType Type { get; set; }

	/// <summary>
	/// ID of the owner of the image (product or category).
	/// </summary>
	public int OwnerId { get; set; }

	/// <summary>
	/// The object key (path) in the storage bucket.
	/// Example: "products/123/a1b2c3d4-image.webp"
	/// </summary>
	public string ObjectKey { get; set; } = null!;

	/// <summary>
	/// Name of the image file.
	/// </summary>
	public string FileName { get; set; } = null!;

	/// <summary>
	/// MIME type of the image file.
	/// </summary>
	public string ContentType { get; set; } = null!;

	/// <summary>
	/// Size of the image file in bytes.
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
	/// Creates a new ImageAsset for the specified owner.
	/// </summary>
	public static ImageAsset Create(
		ImageAssetType type,
		int ownerId,
		string objectKey,
		string fileName,
		string contentType,
		long sizeBytes,
		int displayOrder = 0,
		bool isPrimary = false)
	{
		return new ImageAsset
		{
			Id = Guid.NewGuid(),
			Type = type,
			OwnerId = ownerId,
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
