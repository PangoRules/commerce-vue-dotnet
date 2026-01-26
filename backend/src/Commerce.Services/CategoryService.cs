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
    Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default);

    /// <summary>
    /// Get admin category details (includes parents/children id+name) by id.
    /// </summary>
    Task<CategoryAdminDetailsResponse?> GetCategoryAdminDetailsAsync(int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Get root categories for navigation (active-only by default).
    /// </summary>
    Task<IReadOnlyList<CategoryResponse>> GetRootsAsync(bool includeInactive = false, CancellationToken ct = default);

    /// <summary>
    /// Get children categories for a parent (active-only by default).
    /// </summary>
    Task<IReadOnlyList<CategoryResponse>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, CancellationToken ct = default);

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

public class CategoryService(ICategoryRepository categoriesRepository) : ICategoryService
{
    public async Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default)
    {
        var paged = await categoriesRepository.GetAllCategoriesAsync(queryParams, ct);

        return new PagedResult<CategoryResponse>(
            [.. paged.Items.Select(Mappers.CategoryMapper.ToResponse)],
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

    public async Task<IReadOnlyList<CategoryResponse>> GetRootsAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var roots = await categoriesRepository.GetRootsAsync(includeInactive, ct);
        return [.. roots.Select(Mappers.CategoryMapper.ToResponse)];
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, CancellationToken ct = default)
    {
        var children = await categoriesRepository.GetChildrenAsync(parentCategoryId, includeInactive, ct);
        return [.. children.Select(Mappers.CategoryMapper.ToResponse)];
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
}
