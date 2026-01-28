using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

/// <summary>
/// Seeds default product images for demo/development purposes.
/// Images should be placed in MinIO at: products/{productId}/default.webp
/// </summary>
[ExcludeFromCodeCoverage]
public class ProductImageSeedConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // Map of ProductId -> (ImageId, FileName, Description for reference)
        var seedImages = new (int ProductId, Guid ImageId, string FileName)[]
        {
            // Electronics
            (1001, Guid.Parse("11111111-1001-1001-1001-000000000001"), "smartphone.webp"),
            (1002, Guid.Parse("11111111-1002-1002-1002-000000000001"), "headphones.webp"),
            (1003, Guid.Parse("11111111-1003-1003-1003-000000000001"), "laptop.webp"),

            // Books
            (2001, Guid.Parse("22222222-2001-2001-2001-000000000001"), "clean-code.webp"),
            (2002, Guid.Parse("22222222-2002-2002-2002-000000000001"), "pragmatic-programmer.webp"),
            (2003, Guid.Parse("22222222-2003-2003-2003-000000000001"), "design-patterns.webp"),

            // Home & Kitchen
            (3001, Guid.Parse("33333333-3001-3001-3001-000000000001"), "air-fryer.webp"),
            (3002, Guid.Parse("33333333-3002-3002-3002-000000000001"), "blender.webp"),
            (3003, Guid.Parse("33333333-3003-3003-3003-000000000001"), "coffee-maker.webp"),

            // Clothing
            (4001, Guid.Parse("44444444-4001-4001-4001-000000000001"), "tshirt.webp"),
            (4002, Guid.Parse("44444444-4002-4002-4002-000000000001"), "jeans.webp"),
            (4003, Guid.Parse("44444444-4003-4003-4003-000000000001"), "hoodie.webp"),

            // Sports & Outdoors
            (5001, Guid.Parse("55555555-5001-5001-5001-000000000001"), "yoga-mat.webp"),
            (5002, Guid.Parse("55555555-5002-5002-5002-000000000001"), "dumbbell-set.webp"),
            (5003, Guid.Parse("55555555-5003-5003-5003-000000000001"), "camping-tent.webp"),
        };

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(seedImages.Select(s => new ProductImage
        {
            Id = s.ImageId,
            ProductId = s.ProductId,
            ObjectKey = $"products/{s.ProductId}/{s.FileName}",
            FileName = s.FileName,
            ContentType = "image/webp",
            SizeBytes = 50000, // Placeholder size
            DisplayOrder = 0,
            IsPrimary = true,
            UploadedAt = seedDate
        }));
    }
}
