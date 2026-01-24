using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class SeedCategoriesAndProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Electronic gadgets and devices", true, "Electronics" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Various kinds of books and literature", true, "Books" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Appliances and tools for home and kitchen", true, "Home & Kitchen" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Men's and women's apparel", true, "Clothing" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sports gear and outdoor equipment", true, "Sports & Outdoors" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsActive", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1001, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Latest model smartphone with advanced features", true, "Smartphone", 699.99m, 50 },
                    { 1002, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Noise-canceling over-ear headphones", true, "Wireless Headphones", 199.99m, 35 },
                    { 1003, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight laptop for work and entertainment", true, "Laptop", 1199.99m, 20 },
                    { 2001, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A Handbook of Agile Software Craftsmanship", true, "Clean Code", 34.99m, 100 },
                    { 2002, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Your journey to mastery", true, "The Pragmatic Programmer", 39.99m, 80 },
                    { 2003, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elements of reusable object-oriented software", true, "Design Patterns", 49.99m, 60 },
                    { 3001, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oil-free air fryer with multiple presets", true, "Air Fryer", 129.99m, 40 },
                    { 3002, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-speed blender for smoothies and soups", true, "Blender", 89.99m, 30 },
                    { 3003, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "12-cup programmable coffee maker", true, "Coffee Maker", 79.99m, 45 },
                    { 4001, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "100% cotton men's t-shirt", true, "Men's T-Shirt", 19.99m, 150 },
                    { 4002, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Slim fit women's jeans", true, "Women's Jeans", 49.99m, 90 },
                    { 4003, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Unisex fleece hoodie", true, "Hoodie", 39.99m, 70 },
                    { 5001, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Non-slip yoga mat", true, "Yoga Mat", 29.99m, 60 },
                    { 5002, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Adjustable dumbbell set", true, "Dumbbell Set", 99.99m, 25 },
                    { 5003, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "4-person waterproof camping tent", true, "Camping Tent", 149.99m, 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_Name",
                table: "Category");

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3001);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3002);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3003);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4001);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4002);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4003);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5001);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5002);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5003);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
