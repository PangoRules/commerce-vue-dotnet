
using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Requests;


[ExcludeFromCodeCoverage]
public sealed class GetCategoriesQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; } = null;
    public bool SortDescending {get; set;} = false;
}

[ExcludeFromCodeCoverage]
public class CreateCategoryRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    // If empty or null => root category
    // If has items => attach under these parents
    public List<int>? ParentCategoryIds { get; set; }
}
