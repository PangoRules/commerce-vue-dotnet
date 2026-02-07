using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Commerce.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTranslation", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EntityTranslation",
                columns: new[] { "Id", "Description", "EntityId", "EntityType", "LanguageCode", "Name" },
                values: new object[,]
                {
                    { 1, "Dispositivos y aparatos electrónicos", 1, "Category", "es", "Electrónica" },
                    { 2, "Varios tipos de libros y literatura", 2, "Category", "es", "Libros" },
                    { 3, "Electrodomésticos y herramientas para el hogar y la cocina", 3, "Category", "es", "Hogar y Cocina" },
                    { 4, "Ropa para hombres y mujeres", 4, "Category", "es", "Ropa" },
                    { 5, "Equipamiento deportivo y para actividades al aire libre", 5, "Category", "es", "Deportes y Aire Libre" },
                    { 6, "Escritorios, portátiles, componentes", 6, "Category", "es", "Computadoras" },
                    { 7, "Computadoras portátiles", 7, "Category", "es", "Portátiles" },
                    { 8, "Ropa de hombres", 8, "Category", "es", "Hombres" },
                    { 9, "Ropa de mujeres", 9, "Category", "es", "Mujeres" },
                    { 10, "Camisas y blusas", 10, "Category", "es", "Camisas" },
                    { 11, "Último modelo de teléfono inteligente con funciones avanzadas", 1001, "Product", "es", "Teléfono Inteligente" },
                    { 12, "Auriculares supraaurales con cancelación de ruido", 1002, "Product", "es", "Auriculares Inalámbricos" },
                    { 13, "Portátil ligero para trabajo y entretenimiento", 1003, "Product", "es", "Portátil" },
                    { 14, "Manual de desarrollo de software ágil", 2001, "Product", "es", "Código Limpio" },
                    { 15, "Tu camino hacia la maestría", 2002, "Product", "es", "El Programador Pragmático" },
                    { 16, "Elementos de software orientado a objetos reutilizable", 2003, "Product", "es", "Patrones de Diseño" },
                    { 17, "Freidora de aire sin aceite con múltiples preajustes", 3001, "Product", "es", "Freidora de Aire" },
                    { 18, "Licuadora de alta velocidad para batidos y sopas", 3002, "Product", "es", "Licuadora" },
                    { 19, "Cafetera programable de 12 tazas", 3003, "Product", "es", "Cafetera" },
                    { 20, "Camiseta de hombre 100% algodón", 4001, "Product", "es", "Camiseta de Hombre" },
                    { 21, "Jeans ajustados de mujer", 4002, "Product", "es", "Jeans de Mujer" },
                    { 22, "Sudadera con capucha unisex de forro polar", 4003, "Product", "es", "Sudadera con Capucha" },
                    { 23, "Esterilla de yoga antideslizante", 5001, "Product", "es", "Esterilla de Yoga" },
                    { 24, "Juego de mancuernas ajustables", 5002, "Product", "es", "Juego de Mancuernas" },
                    { 25, "Tienda de campaña impermeable para 4 personas", 5003, "Product", "es", "Tienda de Campaña" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTranslation_EntityType_EntityId_LanguageCode",
                table: "EntityTranslation",
                columns: new[] { "EntityType", "EntityId", "LanguageCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityTranslation");
        }
    }
}
