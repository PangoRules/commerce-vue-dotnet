using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Requests;

namespace Commerce.Repositories.Entities;

[ExcludeFromCodeCoverage]
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Product> Products { get; set; } = [];

    // Graph links:
    // Parents of this category (incoming edges)
    public ICollection<CategoryLink> ParentLinks { get; set; } = [];

    // Children of this category (outgoing edges)
    public ICollection<CategoryLink> ChildLinks { get; set; } = [];

    public void ToggleCategory() => IsActive = !IsActive;

    public static Category FromCreateRequest(CreateCategoryRequest request)
        => new()
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

    public void UpdateCategory(CreateCategoryRequest request)
    {
        Name = request.Name;
        Description = request.Description;
    }
}