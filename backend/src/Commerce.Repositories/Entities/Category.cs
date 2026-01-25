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

    public void ToggleCategory()
    {
        this.IsActive = !this.IsActive;
    }

    public static Category FromCreateRequest(CreateCategoryRequest request)
    {
        return new Category
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateCategory(CreateCategoryRequest request)
    {
        this.Name = request.Name;
        this.Description = request.Description;
    }
}
