using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

[ExcludeFromCodeCoverage]
public class ImageAssetConfiguration : IEntityTypeConfiguration<ImageAsset>
{
    public void Configure(EntityTypeBuilder<ImageAsset> builder)
    {
        builder.ToTable("ImageAssets");

        builder.HasKey(ia => ia.Id);

        builder.Property(ia => ia.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ia => ia.OwnerId)
            .IsRequired();

        builder.Property(ia => ia.ObjectKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ia => ia.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ia => ia.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ia => ia.SizeBytes)
            .IsRequired();

        builder.Property(ia => ia.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ia => ia.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ia => ia.UploadedAt)
            .IsRequired();

        // Composite index for querying images by owner
        builder.HasIndex(ia => new { ia.Type, ia.OwnerId });

        // Composite index for sorted queries by owner
        builder.HasIndex(ia => new { ia.Type, ia.OwnerId, ia.DisplayOrder });

        // Unique constraint on ObjectKey
        builder.HasIndex(ia => ia.ObjectKey)
            .IsUnique();
    }
}
