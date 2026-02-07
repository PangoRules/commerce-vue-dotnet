using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Enums;

namespace Commerce.Repositories.Entities;

[ExcludeFromCodeCoverage]
public class EntityTranslation
{
    public int Id { get; set; }
    public TranslatableEntityType EntityType { get; set; }
    public int EntityId { get; set; }
    public string LanguageCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
