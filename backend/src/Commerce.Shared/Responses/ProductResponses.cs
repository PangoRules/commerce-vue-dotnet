using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Responses;


[ExcludeFromCodeCoverage]
public class ProductResponse
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
    public CategoryResponse? Category { get; set; } = null!;

    /// <summary>
    /// Images associated with this product, ordered by DisplayOrder.
    /// </summary>
    public List<ProductImageResponse> Images { get; set; } = [];

    /// <summary>
    /// URL to the primary image, or null if no primary image is set.
    /// </summary>
    public string? PrimaryImageUrl { get; set; }
}