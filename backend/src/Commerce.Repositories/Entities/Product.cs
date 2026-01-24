using System.Diagnostics.CodeAnalysis;

namespace Commerce.Repositories.Entities;

[ExcludeFromCodeCoverage]
public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public Category Category { get; set; } = null!;
}
