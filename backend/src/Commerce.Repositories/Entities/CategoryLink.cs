using System.Diagnostics.CodeAnalysis;

namespace Commerce.Repositories.Entities;

[ExcludeFromCodeCoverage]
public class CategoryLink
{
    public int ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; } = null!;

    public int ChildCategoryId { get; set; }
    public Category ChildCategory { get; set; } = null!;
}