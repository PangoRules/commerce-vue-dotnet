using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Requests;

/// <summary>
/// Request to reorder images for a product.
/// </summary>
[ExcludeFromCodeCoverage]
public class ReorderImagesRequest
{
    /// <summary>
    /// Image IDs in the desired display order.
    /// The first ID will have DisplayOrder = 0, second = 1, etc.
    /// </summary>
    public required IList<Guid> ImageIds { get; init; }
}
