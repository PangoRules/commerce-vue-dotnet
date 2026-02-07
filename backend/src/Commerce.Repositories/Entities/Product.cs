using Commerce.Shared.Requests;

namespace Commerce.Repositories.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Toggles the active status of the product.
    /// </summary>
    public void ToggleProduct()
    {
        this.IsActive = !this.IsActive;
    }

    /// <summary>
    /// Updates the product with the given request.
    /// </summary>
    /// <param name="request">The request to update the product.</param>
    public void UpdateProduct(UpdateProductRequest request)
    {
        this.Name = request.Name;
        this.Description = request.Description;
        this.Price = request.Price;
        this.SalePrice = request.SalePrice;
        this.StockQuantity = request.StockQuantity;
        this.CategoryId = request.CategoryId;
    }

    /// <summary>
    /// Creates a new product from the given request.
    /// </summary>
    /// <param name="request">The request to create the product.</param>
    /// <returns>A new product created from the request.</returns>
    public static Product FromCreateRequest(CreateProductRequest request)
    {
        return new Product
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            SalePrice = request.SalePrice,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}
