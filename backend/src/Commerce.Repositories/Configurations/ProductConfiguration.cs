using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        #region Keys and Properties
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
        #endregion

        #region Seed Data
        builder.HasData(
            // Electronics
            new Product
            {
                Id = 1001,
                Name = "Smartphone",
                Description = "Latest model smartphone with advanced features",
                Price = 699.99m,
                StockQuantity = 50,
                CategoryId = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 1002,
                Name = "Wireless Headphones",
                Description = "Noise-canceling over-ear headphones",
                Price = 199.99m,
                StockQuantity = 35,
                CategoryId = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 1003,
                Name = "Laptop",
                Description = "Lightweight laptop for work and entertainment",
                Price = 1199.99m,
                StockQuantity = 20,
                CategoryId = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },

            // Books
            new Product
            {
                Id = 2001,
                Name = "Clean Code",
                Description = "A Handbook of Agile Software Craftsmanship",
                Price = 34.99m,
                StockQuantity = 100,
                CategoryId = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 2002,
                Name = "The Pragmatic Programmer",
                Description = "Your journey to mastery",
                Price = 39.99m,
                StockQuantity = 80,
                CategoryId = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 2003,
                Name = "Design Patterns",
                Description = "Elements of reusable object-oriented software",
                Price = 49.99m,
                StockQuantity = 60,
                CategoryId = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },

            // Home & Kitchen
            new Product
            {
                Id = 3001,
                Name = "Air Fryer",
                Description = "Oil-free air fryer with multiple presets",
                Price = 129.99m,
                StockQuantity = 40,
                CategoryId = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 3002,
                Name = "Blender",
                Description = "High-speed blender for smoothies and soups",
                Price = 89.99m,
                StockQuantity = 30,
                CategoryId = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 3003,
                Name = "Coffee Maker",
                Description = "12-cup programmable coffee maker",
                Price = 79.99m,
                StockQuantity = 45,
                CategoryId = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },

            // Clothing
            new Product
            {
                Id = 4001,
                Name = "Men's T-Shirt",
                Description = "100% cotton men's t-shirt",
                Price = 19.99m,
                StockQuantity = 150,
                CategoryId = 4,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 4002,
                Name = "Women's Jeans",
                Description = "Slim fit women's jeans",
                Price = 49.99m,
                StockQuantity = 90,
                CategoryId = 4,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 4003,
                Name = "Hoodie",
                Description = "Unisex fleece hoodie",
                Price = 39.99m,
                StockQuantity = 70,
                CategoryId = 4,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },

            // Sports & Outdoors
            new Product
            {
                Id = 5001,
                Name = "Yoga Mat",
                Description = "Non-slip yoga mat",
                Price = 29.99m,
                StockQuantity = 60,
                CategoryId = 5,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 5002,
                Name = "Dumbbell Set",
                Description = "Adjustable dumbbell set",
                Price = 99.99m,
                StockQuantity = 25,
                CategoryId = 5,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 5003,
                Name = "Camping Tent",
                Description = "4-person waterproof camping tent",
                Price = 149.99m,
                StockQuantity = 15,
                CategoryId = 5,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            }
        );
        #endregion
    }
}
