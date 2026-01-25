using System;

namespace Commerce.Shared.Responses;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
