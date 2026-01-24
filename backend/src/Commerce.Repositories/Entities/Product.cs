using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Requests;

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

    public void ToggleProduct()
    {
        this.IsActive = !this.IsActive;
    }

    public void UpdateProduct(UpdateProductRequest request)
    {
        this.Name = request.Name;
        this.Description = request.Description;
        this.Price = request.Price;
        this.StockQuantity = request.StockQuantity;
        this.CategoryId = request.CategoryId;
    }

    public static Product FromCreateRequest(CreateProductRequest request)
    {
        return new Product
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}
