using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

/// <summary>
/// Seeds default image assets for demo/development purposes.
/// Images should be placed in MinIO at: {type}/{ownerId}/default.webp
/// </summary>
[ExcludeFromCodeCoverage]
public class ImageAssetSeedConfiguration : IEntityTypeConfiguration<ImageAsset>
{
    public void Configure(EntityTypeBuilder<ImageAsset> builder)
    {
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Product images (mirrors ProductImageSeedConfiguration)
        var productImages = new (int OwnerId, Guid ImageId, string FileName)[]
        {
            // Electronics
            (1001, Guid.Parse("aaaa1111-1001-1001-1001-000000000001"), "smartphone.webp"),
            (1002, Guid.Parse("aaaa1111-1002-1002-1002-000000000001"), "headphones.webp"),
            (1003, Guid.Parse("aaaa1111-1003-1003-1003-000000000001"), "laptop.webp"),

            // Books
            (2001, Guid.Parse("aaaa2222-2001-2001-2001-000000000001"), "clean-code.webp"),
            (2002, Guid.Parse("aaaa2222-2002-2002-2002-000000000001"), "pragmatic-programmer.webp"),
            (2003, Guid.Parse("aaaa2222-2003-2003-2003-000000000001"), "design-patterns.webp"),

            // Home & Kitchen
            (3001, Guid.Parse("aaaa3333-3001-3001-3001-000000000001"), "air-fryer.webp"),
            (3002, Guid.Parse("aaaa3333-3002-3002-3002-000000000001"), "blender.webp"),
            (3003, Guid.Parse("aaaa3333-3003-3003-3003-000000000001"), "coffee-maker.webp"),

            // Clothing
            (4001, Guid.Parse("aaaa4444-4001-4001-4001-000000000001"), "tshirt.webp"),
            (4002, Guid.Parse("aaaa4444-4002-4002-4002-000000000001"), "jeans.webp"),
            (4003, Guid.Parse("aaaa4444-4003-4003-4003-000000000001"), "hoodie.webp"),

            // Sports & Outdoors
            (5001, Guid.Parse("aaaa5555-5001-5001-5001-000000000001"), "yoga-mat.webp"),
            (5002, Guid.Parse("aaaa5555-5002-5002-5002-000000000001"), "dumbbell-set.webp"),
            (5003, Guid.Parse("aaaa5555-5003-5003-5003-000000000001"), "camping-tent.webp"),
        };

        // Category images
        var categoryImages = new (int OwnerId, Guid ImageId, string FileName, int DisplayOrder)[]
        {
            // Electronics (Category 1)
            (1, Guid.Parse("bbbb0001-0001-0001-0001-000000000001"), "elec1.jpg", 0),
            (1, Guid.Parse("bbbb0001-0001-0001-0001-000000000002"), "elec2.jpg", 1),
            (1, Guid.Parse("bbbb0001-0001-0001-0001-000000000003"), "elec3.jpg", 2),

            // Books (Category 2)
            (2, Guid.Parse("bbbb0002-0002-0002-0002-000000000001"), "dariuszsankowski-glasses-1052023_640.jpg", 0),
            (2, Guid.Parse("bbbb0002-0002-0002-0002-000000000002"), "dglodowska-book-419589_640.jpg", 1),

            // Home & Kitchen (Category 3)
            (3, Guid.Parse("bbbb0003-0003-0003-0003-000000000001"), "kitchen1.jpg", 0),
            (3, Guid.Parse("bbbb0003-0003-0003-0003-000000000002"), "kitchen2.jpg", 1),
            (3, Guid.Parse("bbbb0003-0003-0003-0003-000000000003"), "kitchen3.jpg", 2),

            // Clothing (Category 4)
            (4, Guid.Parse("bbbb0004-0004-0004-0004-000000000001"), "cloth1.jpg", 0),
            (4, Guid.Parse("bbbb0004-0004-0004-0004-000000000002"), "cloth2.jpg", 1),
            (4, Guid.Parse("bbbb0004-0004-0004-0004-000000000003"), "cloth3.jpg", 2),
            (4, Guid.Parse("bbbb0004-0004-0004-0004-000000000004"), "cloth4.jpg", 3),
        };

        builder.HasData(productImages.Select(s => new ImageAsset
        {
            Id = s.ImageId,
            Type = ImageAssetType.Product,
            OwnerId = s.OwnerId,
            ObjectKey = $"products/{s.OwnerId}/{s.FileName}",
            FileName = s.FileName,
            ContentType = "image/webp",
            SizeBytes = 50000,
            DisplayOrder = 0,
            IsPrimary = true,
            UploadedAt = seedDate
        }));

        builder.HasData(categoryImages.Select(s => new ImageAsset
        {
            Id = s.ImageId,
            Type = ImageAssetType.Category,
            OwnerId = s.OwnerId,
            ObjectKey = $"categories/{s.OwnerId}/{s.FileName}",
            FileName = s.FileName,
            ContentType = "image/jpeg",
            SizeBytes = 50000,
            DisplayOrder = s.DisplayOrder,
            IsPrimary = s.DisplayOrder == 0,
            UploadedAt = seedDate
        }));
    }
}
