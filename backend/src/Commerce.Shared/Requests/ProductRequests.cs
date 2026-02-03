using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Requests;

/// <summary>
/// Base class for product request DTOs containing shared properties.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class ProductRequestBase
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? SalePrice { get; init; }
    public int StockQuantity { get; init; }
}

[ExcludeFromCodeCoverage]
public class CreateProductRequest : ProductRequestBase { }

[ExcludeFromCodeCoverage]
public class UpdateProductRequest : ProductRequestBase { }

public enum ProductSortBy { Name, Price, StockQuantity, CategoryId }

[ExcludeFromCodeCoverage]
public sealed class GetProductsQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; } = true;
    public bool OnSale { get; set; } = false;
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Name;
    public bool SortDescending { get; set; } = false;
}
