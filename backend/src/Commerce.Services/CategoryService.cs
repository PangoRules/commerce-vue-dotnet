using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.Services;

public interface ICategoryService
{
    /// <summary>
    /// Retrieve a paginated list of categories based on query parameters.
    /// </summary>
    Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQueryParams queryParams, string? language = null, CancellationToken ct = default);

    /// <summary>
    /// Get admin category details (includes parents/children id+name) by id.
    /// </summary>
    Task<CategoryAdminDetailsResponse?> GetCategoryAdminDetailsAsync(int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Get root categories for navigation (active-only by default).
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="featuredOnly">Whether to return only featured categories.</param>
    /// <param name="language">Optional language code for translated content.</param>
    /// <param name="ct">The cancellation token.</param>
    Task<IReadOnlyList<CategoryResponse>> GetRootsAsync(bool includeInactive = false, bool featuredOnly = false, string? language = null, CancellationToken ct = default);

    /// <summary>
    /// Get children categories for a parent (active-only by default).
    /// </summary>
    Task<IReadOnlyList<CategoryResponse>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, string? language = null, CancellationToken ct = default);

    /// <summary>
    /// Add a new category (reuses existing by name) and optionally attach it under parent(s).
    /// </summary>
    Task<(DbResultOption Result, int CategoryId)> AddCategoryAsync(CreateCategoryRequest categoryRequest, CancellationToken ct = default);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    Task<DbResultOption> UpdateCategoryAsync(CreateCategoryRequest categoryRequest, int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Toggle the active status of a category.
    /// </summary>
    Task<DbResultOption> ToggleCategoryAsync(int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Attach a child category to a parent category.
    /// </summary>
    Task<DbResultOption> AttachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default);

    /// <summary>
    /// Detach a child category from a parent category.
    /// </summary>
    Task<DbResultOption> DetachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default);
}

public class CategoryService(
    ICategoryRepository categoriesRepository,
    IImageAssetRepository imagesRepo,
    IEntityTranslationRepository translationsRepo
) : ICategoryService
{
    public async Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQueryParams queryParams, string? language = null, CancellationToken ct = default)
    {
        var paged = await categoriesRepository.GetAllCategoriesAsync(queryParams, ct);

        var categoryIds = paged.Items.Select(c => c.Id).ToList();
        var images = await imagesRepo.GetByTypeAndOwnersIdsAsync(ImageAssetType.Category, categoryIds, ct);
        var (productImages, productIdsByCategory) = await FetchProductImageFallbacksAsync(categoryIds, images, ct);
        var translations = await GetTranslationsIfNeeded(categoryIds, language, ct);

        return new PagedResult<CategoryResponse>(
            [.. paged.Items.Select(c =>
                Mappers.CategoryMapper.ToResponse(
                    c,
                    SelectImagesForCategory(c.Id, images, productImages, productIdsByCategory),
                    translations?.FirstOrDefault(t => t.EntityId == c.Id)))],
            paged.Page,
            paged.PageSize,
            paged.TotalCount);
    }

    public async Task<CategoryAdminDetailsResponse?> GetCategoryAdminDetailsAsync(int categoryId, CancellationToken ct = default)
    {
        var category = await categoriesRepository.GetCategoryGraphByIdAsync(categoryId, ct);
        if (category is null) return null;

        return ToAdminDetailsResponse(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetRootsAsync(bool includeInactive = false, bool featuredOnly = false, string? language = null, CancellationToken ct = default)
    {
        var roots = await categoriesRepository.GetRootsAsync(includeInactive, featuredOnly, ct);

        var categoryIds = roots.Select(c => c.Id).ToList();
        var images = await imagesRepo.GetByTypeAndOwnersIdsAsync(ImageAssetType.Category, categoryIds, ct);
        var (productImages, productIdsByCategory) = await FetchProductImageFallbacksAsync(categoryIds, images, ct);
        var translations = await GetTranslationsIfNeeded(categoryIds, language, ct);

        return [.. roots.Select(c =>
            Mappers.CategoryMapper.ToResponse(
                c,
                SelectImagesForCategory(c.Id, images, productImages, productIdsByCategory),
                translations?.FirstOrDefault(t => t.EntityId == c.Id)))];
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, string? language = null, CancellationToken ct = default)
    {
        var children = await categoriesRepository.GetChildrenAsync(parentCategoryId, includeInactive, ct);

        var categoryIds = children.Select(c => c.Id).ToList();
        var images = await imagesRepo.GetByTypeAndOwnersIdsAsync(ImageAssetType.Category, categoryIds, ct);
        var (productImages, productIdsByCategory) = await FetchProductImageFallbacksAsync(categoryIds, images, ct);
        var translations = await GetTranslationsIfNeeded(categoryIds, language, ct);

        return [.. children.Select(c =>
            Mappers.CategoryMapper.ToResponse(
                c,
                SelectImagesForCategory(c.Id, images, productImages, productIdsByCategory),
                translations?.FirstOrDefault(t => t.EntityId == c.Id)))];
    }

    public Task<(DbResultOption Result, int CategoryId)> AddCategoryAsync(CreateCategoryRequest categoryRequest, CancellationToken ct = default)
        => categoriesRepository.AddCategoryAsync(categoryRequest, ct);

    public Task<DbResultOption> UpdateCategoryAsync(CreateCategoryRequest categoryRequest, int categoryId, CancellationToken ct = default)
        => categoriesRepository.UpdateCategoryAsync(categoryRequest, categoryId, ct);

    public Task<DbResultOption> ToggleCategoryAsync(int categoryId, CancellationToken ct = default)
        => categoriesRepository.ToggleCategoryAsync(categoryId, ct);

    public Task<DbResultOption> AttachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
        => categoriesRepository.AttachCategoryAsync(parentCategoryId, childCategoryId, ct);

    public Task<DbResultOption> DetachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
        => categoriesRepository.DetachCategoryAsync(parentCategoryId, childCategoryId, ct);

    /// <summary>
    /// For each category ID that has no category-owned images, fetches the active product IDs
    /// for that category and then fetches their product images. Returns an empty pair when
    /// every category already has at least one image.
    /// </summary>
    private async Task<(List<ImageAsset> ProductImages, Dictionary<int, List<int>> ProductIdsByCategory)>
        FetchProductImageFallbacksAsync(List<int> categoryIds, List<ImageAsset> categoryImages, CancellationToken ct)
    {
        var emptyCategoryIds = categoryIds
            .Where(id => !categoryImages.Any(img => img.OwnerId == id))
            .ToList();

        if (emptyCategoryIds.Count == 0)
            return ([], []);

        var productIdsByCategory = await categoriesRepository.GetProductIdsByCategoryIdsAsync(emptyCategoryIds, ct);
        var allProductIds = productIdsByCategory.Values.SelectMany(ids => ids).Distinct().ToList();

        if (allProductIds.Count == 0)
            return ([], productIdsByCategory);

        var productImages = await imagesRepo.GetByTypeAndOwnersIdsAsync(ImageAssetType.Product, allProductIds, ct);
        return (productImages, productIdsByCategory);
    }

    /// <summary>
    /// Returns the category's own images when available; otherwise falls back to the
    /// product images belonging to that category's products.
    /// </summary>
    private static IEnumerable<ImageAsset> SelectImagesForCategory(
        int categoryId,
        List<ImageAsset> categoryImages,
        List<ImageAsset> productImages,
        Dictionary<int, List<int>> productIdsByCategory)
    {
        var ownImages = categoryImages.Where(i => i.OwnerId == categoryId).ToList();
        if (ownImages.Count > 0) return ownImages;

        if (!productIdsByCategory.TryGetValue(categoryId, out var productIds)) return [];
        return productImages.Where(i => productIds.Contains(i.OwnerId));
    }

    private static CategoryAdminDetailsResponse ToAdminDetailsResponse(Category c)
    {
        // parents = incoming edges: ParentLinks -> ParentCategory
        var parents = c.ParentLinks
            .Select(l => new IdName(l.ParentCategoryId, l.ParentCategory.Name))
            .OrderBy(x => x.Name)
            .ToList();

        // children = outgoing edges: ChildLinks -> ChildCategory
        var children = c.ChildLinks
            .Select(l => new IdName(l.ChildCategoryId, l.ChildCategory.Name))
            .OrderBy(x => x.Name)
            .ToList();

        return new CategoryAdminDetailsResponse(
            c.Id,
            c.Name,
            c.Description,
            c.IsActive,
            parents,
            children
        );
    }

    private static bool ShouldTranslate(string? language)
        => !string.IsNullOrWhiteSpace(language) && !language.Equals("en", StringComparison.OrdinalIgnoreCase);

    private async Task<IReadOnlyList<EntityTranslation>?> GetTranslationsIfNeeded(
        List<int> entityIds, string? language, CancellationToken ct)
    {
        if (!ShouldTranslate(language)) return null;
        return await translationsRepo.GetTranslationsAsync(TranslatableEntityType.Category, entityIds, language!, ct);
    }
}
