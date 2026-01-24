using System.Diagnostics.CodeAnalysis;

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
}
