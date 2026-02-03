using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddSalePriceAndIsFeatured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "Product",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Category",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Set featured categories
            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsFeatured",
                value: true);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsFeatured",
                value: true);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsFeatured",
                value: true);

            // Set sale prices for specific products
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1001,
                column: "SalePrice",
                value: 599.99m);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1002,
                column: "SalePrice",
                value: 149.99m);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3001,
                column: "SalePrice",
                value: 99.99m);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4001,
                column: "SalePrice",
                value: 14.99m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Category");
        }
    }
}
