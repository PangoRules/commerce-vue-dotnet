using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Requests;

[ExcludeFromCodeCoverage]
public class CreateProductRequest
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
}


[ExcludeFromCodeCoverage]
public class UpdateProductRequest
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
}