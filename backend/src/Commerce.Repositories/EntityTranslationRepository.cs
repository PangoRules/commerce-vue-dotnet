using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;

public interface IEntityTranslationRepository
{
    Task<EntityTranslation?> GetTranslationAsync(
        TranslatableEntityType entityType, int entityId,
        string languageCode, CancellationToken ct = default);

    Task<IReadOnlyList<EntityTranslation>> GetTranslationsAsync(
        TranslatableEntityType entityType, IEnumerable<int> entityIds,
        string languageCode, CancellationToken ct = default);
}

[ExcludeFromCodeCoverage]
public class EntityTranslationRepository(CommerceDbContext context) : IEntityTranslationRepository
{
    public async Task<EntityTranslation?> GetTranslationAsync(
        TranslatableEntityType entityType, int entityId,
        string languageCode, CancellationToken ct = default)
    {
        return await context.EntityTranslations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                et => et.EntityType == entityType
                      && et.EntityId == entityId
                      && et.LanguageCode == languageCode,
                ct);
    }

    public async Task<IReadOnlyList<EntityTranslation>> GetTranslationsAsync(
        TranslatableEntityType entityType, IEnumerable<int> entityIds,
        string languageCode, CancellationToken ct = default)
    {
        var ids = entityIds.ToList();
        return await context.EntityTranslations
            .AsNoTracking()
            .Where(et => et.EntityType == entityType
                         && ids.Contains(et.EntityId)
                         && et.LanguageCode == languageCode)
            .ToListAsync(ct);
    }
}
