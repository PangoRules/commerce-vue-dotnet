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

        builder.HasIndex(c => c.Name).IsUnique();

        // Seed moved/expanded below
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic gadgets and devices", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 2, Name = "Books", Description = "Various kinds of books and literature", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 3, Name = "Home & Kitchen", Description = "Appliances and tools for home and kitchen", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 4, Name = "Clothing", Description = "Men's and women's apparel", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 5, Name = "Sports & Outdoors", Description = "Sports gear and outdoor equipment", CreatedAt = seedDate, IsActive = true },

            // Shared nodes
            new Category { Id = 6, Name = "Computers", Description = "Desktops, laptops, components", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 7, Name = "Laptops", Description = "Portable computers", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 8, Name = "Men", Description = "Men's clothing", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 9, Name = "Women", Description = "Women's clothing", CreatedAt = seedDate, IsActive = true },
            new Category { Id = 10, Name = "Shirts", Description = "Shirts & tops", CreatedAt = seedDate, IsActive = true }
        );
    }
}