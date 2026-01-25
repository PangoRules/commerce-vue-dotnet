using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

[ExcludeFromCodeCoverage]
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        #region Keys and Properties
        builder.ToTable("Category");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);
        #endregion

        #region Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();
        #endregion

        #region Seed Data
        builder.HasData(
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic gadgets and devices",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Category
            {
                Id = 2,
                Name = "Books",
                Description = "Various kinds of books and literature",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Category
            {
                Id = 3,
                Name = "Home & Kitchen",
                Description = "Appliances and tools for home and kitchen",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Category
            {
                Id = 4,
                Name = "Clothing",
                Description = "Men's and women's apparel",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Category
            {
                Id = 5,
                Name = "Sports & Outdoors",
                Description = "Sports gear and outdoor equipment",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            }
        );
        #endregion
    }
}
