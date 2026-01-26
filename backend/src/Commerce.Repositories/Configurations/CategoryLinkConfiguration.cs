using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Repositories.Configurations;

[ExcludeFromCodeCoverage]
public class CategoryLinkConfiguration : IEntityTypeConfiguration<CategoryLink>
{
    public void Configure(EntityTypeBuilder<CategoryLink> builder)
    {
        builder.ToTable(
            "CategoryLink",
            t => t.HasCheckConstraint(
                "CK_CategoryLink_NoSelfLink",
                "\"ParentCategoryId\" <> \"ChildCategoryId\""
            )
        );

        builder.HasKey(x => new { x.ParentCategoryId, x.ChildCategoryId });

        builder.HasOne(x => x.ParentCategory)
            .WithMany(c => c.ChildLinks)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChildCategory)
            .WithMany(c => c.ParentLinks)
            .HasForeignKey(x => x.ChildCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            // Electronics > Computers > Laptops
            new CategoryLink { ParentCategoryId = 1, ChildCategoryId = 6 },
            new CategoryLink { ParentCategoryId = 6, ChildCategoryId = 7 },

            // Clothing > Men, Women
            new CategoryLink { ParentCategoryId = 4, ChildCategoryId = 8 },
            new CategoryLink { ParentCategoryId = 4, ChildCategoryId = 9 },

            // Shared category: Shirts under BOTH Men and Women
            new CategoryLink { ParentCategoryId = 8, ChildCategoryId = 10 },
            new CategoryLink { ParentCategoryId = 9, ChildCategoryId = 10 }
        );
    }
}