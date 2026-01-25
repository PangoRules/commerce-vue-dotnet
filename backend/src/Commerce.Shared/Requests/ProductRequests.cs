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

public enum ProductSortBy { Name, Price, StockQuantity, CategoryId }

[ExcludeFromCodeCoverage]
public sealed class GetProductsQueryParams
{
    public int Page {get; set;} = 1;
    public int PageSize {get; set;} = 10;
    public string? SearchTerm {get; set;}
    public int? CategoryId {get; set;}
    public bool? IsActive {get; set;} = true; 
    public ProductSortBy SortBy {get; set;} = ProductSortBy.Name;
    public bool SortDescending {get; set;} = false;
}