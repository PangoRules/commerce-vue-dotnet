using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Responses;

[ExcludeFromCodeCoverage]
public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed record IdName(int Id, string Name);

[ExcludeFromCodeCoverage]
public sealed record CategoryAdminDetailsResponse(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    IReadOnlyList<IdName> Parents,
    IReadOnlyList<IdName> Children
);