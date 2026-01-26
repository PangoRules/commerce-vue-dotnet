using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddCategoryHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryLink",
                columns: table => new
                {
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    ChildCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryLink", x => new { x.ParentCategoryId, x.ChildCategoryId });
                    table.CheckConstraint("CK_CategoryLink_NoSelfLink", "\"ParentCategoryId\" <> \"ChildCategoryId\"");
                    table.ForeignKey(
                        name: "FK_CategoryLink_Category_ChildCategoryId",
                        column: x => x.ChildCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryLink_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desktops, laptops, components", true, "Computers" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Portable computers", true, "Laptops" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Men's clothing", true, "Men" },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Women's clothing", true, "Women" },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shirts & tops", true, "Shirts" }
                });

            migrationBuilder.InsertData(
                table: "CategoryLink",
                columns: new[] { "ChildCategoryId", "ParentCategoryId" },
                values: new object[,]
                {
                    { 6, 1 },
                    { 8, 4 },
                    { 9, 4 },
                    { 7, 6 },
                    { 10, 8 },
                    { 10, 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLink_ChildCategoryId",
                table: "CategoryLink",
                column: "ChildCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryLink");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
