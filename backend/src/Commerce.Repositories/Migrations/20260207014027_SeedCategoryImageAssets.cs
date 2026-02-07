using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoryImageAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "FileName", "IsPrimary", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("aaaa1111-1001-1001-1001-000000000001"), "image/webp", "smartphone.webp", true, "products/1001/smartphone.webp", 1001, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa1111-1002-1002-1002-000000000001"), "image/webp", "headphones.webp", true, "products/1002/headphones.webp", 1002, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa1111-1003-1003-1003-000000000001"), "image/webp", "laptop.webp", true, "products/1003/laptop.webp", 1003, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa2222-2001-2001-2001-000000000001"), "image/webp", "clean-code.webp", true, "products/2001/clean-code.webp", 2001, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa2222-2002-2002-2002-000000000001"), "image/webp", "pragmatic-programmer.webp", true, "products/2002/pragmatic-programmer.webp", 2002, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa2222-2003-2003-2003-000000000001"), "image/webp", "design-patterns.webp", true, "products/2003/design-patterns.webp", 2003, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa3333-3001-3001-3001-000000000001"), "image/webp", "air-fryer.webp", true, "products/3001/air-fryer.webp", 3001, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa3333-3002-3002-3002-000000000001"), "image/webp", "blender.webp", true, "products/3002/blender.webp", 3002, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa3333-3003-3003-3003-000000000001"), "image/webp", "coffee-maker.webp", true, "products/3003/coffee-maker.webp", 3003, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa4444-4001-4001-4001-000000000001"), "image/webp", "tshirt.webp", true, "products/4001/tshirt.webp", 4001, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa4444-4002-4002-4002-000000000001"), "image/webp", "jeans.webp", true, "products/4002/jeans.webp", 4002, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa4444-4003-4003-4003-000000000001"), "image/webp", "hoodie.webp", true, "products/4003/hoodie.webp", 4003, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa5555-5001-5001-5001-000000000001"), "image/webp", "yoga-mat.webp", true, "products/5001/yoga-mat.webp", 5001, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa5555-5002-5002-5002-000000000001"), "image/webp", "dumbbell-set.webp", true, "products/5002/dumbbell-set.webp", 5002, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaa5555-5003-5003-5003-000000000001"), "image/webp", "camping-tent.webp", true, "products/5003/camping-tent.webp", 5003, 50000L, "Product", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbb0001-0001-0001-0001-000000000001"), "image/jpeg", "elec1.jpg", true, "categories/1/elec1.jpg", 1, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "DisplayOrder", "FileName", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("bbbb0001-0001-0001-0001-000000000002"), "image/jpeg", 1, "elec2.jpg", "categories/1/elec2.jpg", 1, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbb0001-0001-0001-0001-000000000003"), "image/jpeg", 2, "elec3.jpg", "categories/1/elec3.jpg", 1, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "FileName", "IsPrimary", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[] { new Guid("bbbb0002-0002-0002-0002-000000000001"), "image/jpeg", "dariuszsankowski-glasses-1052023_640.jpg", true, "categories/2/dariuszsankowski-glasses-1052023_640.jpg", 2, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "DisplayOrder", "FileName", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[] { new Guid("bbbb0002-0002-0002-0002-000000000002"), "image/jpeg", 1, "dglodowska-book-419589_640.jpg", "categories/2/dglodowska-book-419589_640.jpg", 2, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "FileName", "IsPrimary", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[] { new Guid("bbbb0003-0003-0003-0003-000000000001"), "image/jpeg", "kitchen1.jpg", true, "categories/3/kitchen1.jpg", 3, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "DisplayOrder", "FileName", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("bbbb0003-0003-0003-0003-000000000002"), "image/jpeg", 1, "kitchen2.jpg", "categories/3/kitchen2.jpg", 3, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbb0003-0003-0003-0003-000000000003"), "image/jpeg", 2, "kitchen3.jpg", "categories/3/kitchen3.jpg", 3, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "FileName", "IsPrimary", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[] { new Guid("bbbb0004-0004-0004-0004-000000000001"), "image/jpeg", "cloth1.jpg", true, "categories/4/cloth1.jpg", 4, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "ImageAssets",
                columns: new[] { "Id", "ContentType", "DisplayOrder", "FileName", "ObjectKey", "OwnerId", "SizeBytes", "Type", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("bbbb0004-0004-0004-0004-000000000002"), "image/jpeg", 1, "cloth2.jpg", "categories/4/cloth2.jpg", 4, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbb0004-0004-0004-0004-000000000003"), "image/jpeg", 2, "cloth3.jpg", "categories/4/cloth3.jpg", 4, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbb0004-0004-0004-0004-000000000004"), "image/jpeg", 3, "cloth4.jpg", "categories/4/cloth4.jpg", 4, 50000L, "Category", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1001-1001-1001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1002-1002-1002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1003-1003-1003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2001-2001-2001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2002-2002-2002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2003-2003-2003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3001-3001-3001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3002-3002-3002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3003-3003-3003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4001-4001-4001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4002-4002-4002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4003-4003-4003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa5555-5001-5001-5001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa5555-5002-5002-5002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("aaaa5555-5003-5003-5003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0002-0002-0002-0002-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0002-0002-0002-0002-000000000002"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0003-0003-0003-0003-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0003-0003-0003-0003-000000000002"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0003-0003-0003-0003-000000000003"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0004-0004-0004-0004-000000000001"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0004-0004-0004-0004-000000000002"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0004-0004-0004-0004-000000000003"));

            migrationBuilder.DeleteData(
                table: "ImageAssets",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0004-0004-0004-0004-000000000004"));

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ObjectKey",
                table: "ProductImage",
                column: "ObjectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId_IsPrimary",
                table: "ProductImage",
                columns: new[] { "ProductId", "IsPrimary" },
                filter: "\"IsPrimary\" = true");
        }
    }
}
