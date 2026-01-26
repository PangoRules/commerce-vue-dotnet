using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// Get all categories filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter categories.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A paged result of categories.</returns>
    Task<PagedResult<Category>> GetAllCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default);

    /// <summary>
    /// Get category admin details by ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The category admin details response if found, otherwise null.</returns>
    Task<Category?> GetCategoryGraphByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Simple get category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The category if found, otherwise null.</returns>
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);


    /// <summary>
    /// Get all root categories (categories without parents).
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A list of root categories.</returns>
    Task<IReadOnlyList<Category>> GetRootsAsync(bool includeInactive = false, CancellationToken ct = default);

    /// <summary>
    /// Get all child categories of a given parent category.
    /// </summary>
    /// <param name="parentCategoryId">The ID of the parent category.</param>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A list of child categories.</returns>
    Task<IReadOnlyList<Category>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, CancellationToken ct = default);

    /// <summary>
    /// Add a new category if it doesn't already exist.
    /// </summary>
    /// <param name="category">The category to be added.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation and the category ID.</returns>
    Task<(DbResultOption Result, int CategoryId)> AddCategoryAsync(CreateCategoryRequest category, CancellationToken ct = default);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="category">The category to be updated.</param>
    /// <param name="categoryId">The ID of the category to update.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> UpdateCategoryAsync(CreateCategoryRequest category, int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Toggle the active status of a category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to toggle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> ToggleCategoryAsync(int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Attach a child category to a parent category.
    /// </summary>
    /// <param name="parentCategoryId">The ID of the parent category.</param>
    /// <param name="childCategoryId">The ID of the child category.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> AttachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default);

    /// <summary>
    /// Detach a child category from a parent category.
    /// </summary>
    /// <param name="parentCategoryId">The ID of the parent category.</param>
    /// <param name="childCategoryId">The ID of the child category.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> DetachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default);
}

[ExcludeFromCodeCoverage]
public class CategoryRepository(CommerceDbContext context) : ICategoryRepository
{
    public async Task<PagedResult<Category>> GetAllCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default)
    {
        var query = context.Categories.AsNoTracking().AsQueryable();

        if(!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(queryParams.SearchTerm));
        }

        if(queryParams.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == queryParams.IsActive.Value);
        }

        var totalItems = await query.CountAsync(ct);

        if(queryParams.SortDescending)
        {
            query = query.OrderByDescending(c => c.Name);
        }
        else
        {
            query = query.OrderBy(c => c.Name);
        }

        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Category>(
            items,
            queryParams.Page,
            queryParams.PageSize,
            totalItems
        );
    }

    public async Task<Category?> GetCategoryGraphByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Include(c => c.ParentLinks).ThenInclude(l => l.ParentCategory)
            .Include(c => c.ChildLinks).ThenInclude(l => l.ChildCategory)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Category>> GetRootsAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var query = context.Categories.AsNoTracking()
            .Where(c => !c.ParentLinks.Any());

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query.OrderBy(c => c.Name).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, CancellationToken ct = default)
    {
        var query = context.Set<CategoryLink>()
            .AsNoTracking()
            .Where(x => x.ParentCategoryId == parentCategoryId)
            .Select(x => x.ChildCategory);

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query.OrderBy(c => c.Name).ToListAsync(ct);
    }
    
    public async Task<(DbResultOption Result, int CategoryId)> AddCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        // 1) Find or create the Category node (global unique by Name)
        var category = await context.Categories
            .FirstOrDefaultAsync(c => EF.Functions.ILike(c.Name, request.Name), ct);

        if (category == null)
        {
            try
            {
                category = Category.FromCreateRequest(request);
                await context.Categories.AddAsync(category, ct);
                await context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return (DbResultOption.Conflict, 0);
            }
        }

        // 2) Attach as root or child(ren)
        var parentIds = request.ParentCategoryIds?
            .Distinct()
            .Where(id => id > 0)
            .ToList();

        if (parentIds == null || parentIds.Count == 0)
        {
            // Root category = no links needed
            return (DbResultOption.Success, category.Id);
        }

        // Validate parents exist
        var existingParents = await context.Categories
            .Where(c => parentIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (existingParents.Count != parentIds.Count)
        {
            // At least one parent doesn't exist
            return (DbResultOption.NotFound, 0);
        }

        // Create links that don't already exist
        var existingLinks = await context.Set<CategoryLink>()
            .Where(x => x.ChildCategoryId == category.Id && parentIds.Contains(x.ParentCategoryId))
            .Select(x => x.ParentCategoryId)
            .ToListAsync(ct);

        var newLinks = parentIds
            .Except(existingLinks)
            .Select(parentId => new CategoryLink
            {
                ParentCategoryId = parentId,
                ChildCategoryId = category.Id
            })
            .ToList();

        if (newLinks.Count == 0) return (DbResultOption.Success, category.Id);

        try
        {
            await context.Set<CategoryLink>().AddRangeAsync(newLinks, ct);
            await context.SaveChangesAsync(ct);
            return (DbResultOption.Success, category.Id);
        }
        catch (DbUpdateException)
        {
            return (DbResultOption.Conflict, 0);
        }
    }

    public async Task<DbResultOption> UpdateCategoryAsync(CreateCategoryRequest categoryRequest, int categoryId, CancellationToken ct = default)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        if (category == null) return DbResultOption.NotFound;

        category.UpdateCategory(categoryRequest);
        var result = await context.SaveChangesAsync(ct);
        return result > 0 ? DbResultOption.Success : DbResultOption.Error;
    }

    public async Task<DbResultOption> ToggleCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        if (category == null) return DbResultOption.NotFound;

        category.ToggleCategory();
        var result = await context.SaveChangesAsync(ct);
        return result > 0 ? DbResultOption.Success : DbResultOption.Error;
    }

    public async Task<DbResultOption> AttachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
    {
        // Check if link already exists
        var existingLink = await context.Set<CategoryLink>()
            .AnyAsync(x => x.ParentCategoryId == parentCategoryId && x.ChildCategoryId == childCategoryId, ct);

        if (existingLink)
        {
            // Link already exists
            return DbResultOption.AlreadyExists;
        }

        if (parentCategoryId == childCategoryId) return DbResultOption.Invalid;
        var bothExist = await context.Categories
            .CountAsync(c => c.Id == parentCategoryId || c.Id == childCategoryId, ct);

        if (bothExist != 2) return DbResultOption.NotFound;

        var newLink = new CategoryLink
        {
            ParentCategoryId = parentCategoryId,
            ChildCategoryId = childCategoryId
        };

        try
        {
            await context.Set<CategoryLink>().AddAsync(newLink, ct);
            return await context.SaveChangesAsync(ct) > 0 ? DbResultOption.Success : DbResultOption.Error;
        }
        catch (DbUpdateException)
        {
            return DbResultOption.Conflict;
        }
    }

    public async Task<DbResultOption> DetachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
    {
        var existingLink = await context.Set<CategoryLink>()
            .FirstOrDefaultAsync(x => x.ParentCategoryId == parentCategoryId && x.ChildCategoryId == childCategoryId, ct);

        if (existingLink == null)
        {
            // Link does not exist
            return DbResultOption.Success;
        }

        context.Set<CategoryLink>().Remove(existingLink);
        var result = await context.SaveChangesAsync(ct);
        return result > 0 ? DbResultOption.Success : DbResultOption.Error;
    }
}
