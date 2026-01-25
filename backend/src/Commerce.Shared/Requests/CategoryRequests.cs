
using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Requests;


[ExcludeFromCodeCoverage]
public sealed class GetCategoriesQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; } = true;
    public bool SortDescending {get; set;} = false;
}

[ExcludeFromCodeCoverage]
public class CreateCategoryRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}
