using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductImage",
                columns: new[] { "Id", "ContentType", "FileName", "IsPrimary", "ObjectKey", "ProductId", "SizeBytes", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1001-1001-1001-000000000001"), "image/webp", "smartphone.webp", true, "products/1001/smartphone.webp", 1001, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1002-1002-1002-000000000001"), "image/webp", "headphones.webp", true, "products/1002/headphones.webp", 1002, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1003-1003-1003-000000000001"), "image/webp", "laptop.webp", true, "products/1003/laptop.webp", 1003, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2001-2001-2001-000000000001"), "image/webp", "clean-code.webp", true, "products/2001/clean-code.webp", 2001, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2002-2002-2002-000000000001"), "image/webp", "pragmatic-programmer.webp", true, "products/2002/pragmatic-programmer.webp", 2002, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2003-2003-2003-000000000001"), "image/webp", "design-patterns.webp", true, "products/2003/design-patterns.webp", 2003, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3001-3001-3001-000000000001"), "image/webp", "air-fryer.webp", true, "products/3001/air-fryer.webp", 3001, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3002-3002-3002-000000000001"), "image/webp", "blender.webp", true, "products/3002/blender.webp", 3002, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3003-3003-3003-000000000001"), "image/webp", "coffee-maker.webp", true, "products/3003/coffee-maker.webp", 3003, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4001-4001-4001-000000000001"), "image/webp", "tshirt.webp", true, "products/4001/tshirt.webp", 4001, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4002-4002-4002-000000000001"), "image/webp", "jeans.webp", true, "products/4002/jeans.webp", 4002, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4003-4003-4003-000000000001"), "image/webp", "hoodie.webp", true, "products/4003/hoodie.webp", 4003, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-5001-5001-5001-000000000001"), "image/webp", "yoga-mat.webp", true, "products/5001/yoga-mat.webp", 5001, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-5002-5002-5002-000000000001"), "image/webp", "dumbbell-set.webp", true, "products/5002/dumbbell-set.webp", 5002, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-5003-5003-5003-000000000001"), "image/webp", "camping-tent.webp", true, "products/5003/camping-tent.webp", 5003, 50000L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1001-1001-1001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1002-1002-1002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1003-1003-1003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2001-2001-2001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2002-2002-2002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2003-2003-2003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3001-3001-3001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3002-3002-3002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3003-3003-3003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4001-4001-4001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4002-4002-4002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4003-4003-4003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5001-5001-5001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5002-5002-5002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImage",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5003-5003-5003-000000000001"));
        }
    }
}
