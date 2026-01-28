using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

[ExcludeFromCodeCoverage]
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImage");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ObjectKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pi => pi.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(pi => pi.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pi => pi.SizeBytes)
            .IsRequired();

        builder.Property(pi => pi.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pi => pi.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pi => pi.UploadedAt)
            .IsRequired();

        // Relationship: ProductImage -> Product
        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for efficient queries by product
        builder.HasIndex(pi => pi.ProductId);

        // Index for finding primary images quickly
        builder.HasIndex(pi => new { pi.ProductId, pi.IsPrimary })
            .HasFilter("\"IsPrimary\" = true");

        // Unique constraint on ObjectKey (each storage path should be unique)
        builder.HasIndex(pi => pi.ObjectKey)
            .IsUnique();
    }
}
