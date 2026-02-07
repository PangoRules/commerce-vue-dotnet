using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

[ExcludeFromCodeCoverage]
public class EntityTranslationConfiguration : IEntityTypeConfiguration<EntityTranslation>
{
    public void Configure(EntityTypeBuilder<EntityTranslation> builder)
    {
        builder.ToTable("EntityTranslation");

        builder.HasKey(et => et.Id);

        builder.Property(et => et.EntityType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(et => et.EntityId)
            .IsRequired();

        builder.Property(et => et.LanguageCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(et => et.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(et => et.Description)
            .HasMaxLength(500);

        // One translation per entity per language
        builder.HasIndex(et => new { et.EntityType, et.EntityId, et.LanguageCode })
            .IsUnique();

        #region Seed Data - Spanish translations

        builder.HasData(
            // Categories
            new EntityTranslation { Id = 1, EntityType = TranslatableEntityType.Category, EntityId = 1, LanguageCode = "es", Name = "Electrónica", Description = "Dispositivos y aparatos electrónicos" },
            new EntityTranslation { Id = 2, EntityType = TranslatableEntityType.Category, EntityId = 2, LanguageCode = "es", Name = "Libros", Description = "Varios tipos de libros y literatura" },
            new EntityTranslation { Id = 3, EntityType = TranslatableEntityType.Category, EntityId = 3, LanguageCode = "es", Name = "Hogar y Cocina", Description = "Electrodomésticos y herramientas para el hogar y la cocina" },
            new EntityTranslation { Id = 4, EntityType = TranslatableEntityType.Category, EntityId = 4, LanguageCode = "es", Name = "Ropa", Description = "Ropa para hombres y mujeres" },
            new EntityTranslation { Id = 5, EntityType = TranslatableEntityType.Category, EntityId = 5, LanguageCode = "es", Name = "Deportes y Aire Libre", Description = "Equipamiento deportivo y para actividades al aire libre" },
            new EntityTranslation { Id = 6, EntityType = TranslatableEntityType.Category, EntityId = 6, LanguageCode = "es", Name = "Computadoras", Description = "Escritorios, portátiles, componentes" },
            new EntityTranslation { Id = 7, EntityType = TranslatableEntityType.Category, EntityId = 7, LanguageCode = "es", Name = "Portátiles", Description = "Computadoras portátiles" },
            new EntityTranslation { Id = 8, EntityType = TranslatableEntityType.Category, EntityId = 8, LanguageCode = "es", Name = "Hombres", Description = "Ropa de hombres" },
            new EntityTranslation { Id = 9, EntityType = TranslatableEntityType.Category, EntityId = 9, LanguageCode = "es", Name = "Mujeres", Description = "Ropa de mujeres" },
            new EntityTranslation { Id = 10, EntityType = TranslatableEntityType.Category, EntityId = 10, LanguageCode = "es", Name = "Camisas", Description = "Camisas y blusas" },

            // Products - Electronics
            new EntityTranslation { Id = 11, EntityType = TranslatableEntityType.Product, EntityId = 1001, LanguageCode = "es", Name = "Teléfono Inteligente", Description = "Último modelo de teléfono inteligente con funciones avanzadas" },
            new EntityTranslation { Id = 12, EntityType = TranslatableEntityType.Product, EntityId = 1002, LanguageCode = "es", Name = "Auriculares Inalámbricos", Description = "Auriculares supraaurales con cancelación de ruido" },
            new EntityTranslation { Id = 13, EntityType = TranslatableEntityType.Product, EntityId = 1003, LanguageCode = "es", Name = "Portátil", Description = "Portátil ligero para trabajo y entretenimiento" },

            // Products - Books
            new EntityTranslation { Id = 14, EntityType = TranslatableEntityType.Product, EntityId = 2001, LanguageCode = "es", Name = "Código Limpio", Description = "Manual de desarrollo de software ágil" },
            new EntityTranslation { Id = 15, EntityType = TranslatableEntityType.Product, EntityId = 2002, LanguageCode = "es", Name = "El Programador Pragmático", Description = "Tu camino hacia la maestría" },
            new EntityTranslation { Id = 16, EntityType = TranslatableEntityType.Product, EntityId = 2003, LanguageCode = "es", Name = "Patrones de Diseño", Description = "Elementos de software orientado a objetos reutilizable" },

            // Products - Home & Kitchen
            new EntityTranslation { Id = 17, EntityType = TranslatableEntityType.Product, EntityId = 3001, LanguageCode = "es", Name = "Freidora de Aire", Description = "Freidora de aire sin aceite con múltiples preajustes" },
            new EntityTranslation { Id = 18, EntityType = TranslatableEntityType.Product, EntityId = 3002, LanguageCode = "es", Name = "Licuadora", Description = "Licuadora de alta velocidad para batidos y sopas" },
            new EntityTranslation { Id = 19, EntityType = TranslatableEntityType.Product, EntityId = 3003, LanguageCode = "es", Name = "Cafetera", Description = "Cafetera programable de 12 tazas" },

            // Products - Clothing
            new EntityTranslation { Id = 20, EntityType = TranslatableEntityType.Product, EntityId = 4001, LanguageCode = "es", Name = "Camiseta de Hombre", Description = "Camiseta de hombre 100% algodón" },
            new EntityTranslation { Id = 21, EntityType = TranslatableEntityType.Product, EntityId = 4002, LanguageCode = "es", Name = "Jeans de Mujer", Description = "Jeans ajustados de mujer" },
            new EntityTranslation { Id = 22, EntityType = TranslatableEntityType.Product, EntityId = 4003, LanguageCode = "es", Name = "Sudadera con Capucha", Description = "Sudadera con capucha unisex de forro polar" },

            // Products - Sports & Outdoors
            new EntityTranslation { Id = 23, EntityType = TranslatableEntityType.Product, EntityId = 5001, LanguageCode = "es", Name = "Esterilla de Yoga", Description = "Esterilla de yoga antideslizante" },
            new EntityTranslation { Id = 24, EntityType = TranslatableEntityType.Product, EntityId = 5002, LanguageCode = "es", Name = "Juego de Mancuernas", Description = "Juego de mancuernas ajustables" },
            new EntityTranslation { Id = 25, EntityType = TranslatableEntityType.Product, EntityId = 5003, LanguageCode = "es", Name = "Tienda de Campaña", Description = "Tienda de campaña impermeable para 4 personas" }
        );

        #endregion
    }
}
