using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class ReassignProductsToChildCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Laptop: Electronics (1) → Computers (6)
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1003,
                column: "CategoryId",
                value: 6);

            // Men's T-Shirt: Clothing (4) → Men (8)
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4001,
                column: "CategoryId",
                value: 8);

            // Women's Jeans: Clothing (4) → Women (9)
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4002,
                column: "CategoryId",
                value: 9);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Laptop: Computers (6) → Electronics (1)
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1003,
                column: "CategoryId",
                value: 1);

            // Men's T-Shirt: Men (8) → Clothing (4)
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4001,
                column: "CategoryId",
                value: 4);

            // Women's Jeans: Women (9) → Clothing (4)
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4002,
                column: "CategoryId",
                value: 4);
        }
    }
}
