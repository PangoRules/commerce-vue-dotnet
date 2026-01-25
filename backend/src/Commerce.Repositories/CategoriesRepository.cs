using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;

public interface ICategoriesRepository
{
    /// <summary>
    /// Get all categories filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter categories.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A paged result of categories.</returns>
    public Task<PagedResult<Category>> GetAllCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default);

    /// <summary>
    /// Add a new category if it doesn't already exist.
    /// </summary>
    /// <param name="category">The category to be added.</param>
    /// <returns>True if the category was added successfully, otherwise false.</returns>
    public Task<bool> AddCategoryAsync(CreateCategoryRequest category);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="category">The category to be updated.</param>
    /// <returns>True if the category was updated successfully, otherwise false.</returns>
    public Task<bool> UpdateCategoryAsync(CreateCategoryRequest category, int categoryId);

    /// <summary>
    /// Toggle the active status of a category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to toggle.</param>
    /// <returns>True if the category was toggled successfully, otherwise false.</returns>
    public Task<bool> ToggleCategoryAsync(int categoryId);
}

public class CategoriesRepository(CommerceDbContext context) : ICategoriesRepository
{
    public Task<PagedResult<Category>> GetAllCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default)
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

        var totalItems = query.Count();

        if(queryParams.SortDescending)
        {
            query = query.OrderByDescending(c => c.Name);
        }
        else
        {
            query = query.OrderBy(c => c.Name);
        }

        var items = query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Category>(items, totalItems, queryParams.Page, queryParams.PageSize));
    }

    public async Task<bool> AddCategoryAsync(CreateCategoryRequest categoryRequest)
    {
        var existingCategory = await context.Categories
            .FirstOrDefaultAsync(c => string.Equals(c.Name, categoryRequest.Name, StringComparison.OrdinalIgnoreCase));
        
        if (existingCategory != null) return false;

        var newCategory = Category.FromCreateRequest(categoryRequest);
        await context.Categories.AddAsync(newCategory);
        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateCategoryAsync(CreateCategoryRequest categoryRequest, int categoryId)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category == null) return false;

        category.UpdateCategory(categoryRequest);
        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ToggleCategoryAsync(int categoryId)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category == null) return false;

        category.ToggleCategory();
        var result = await context.SaveChangesAsync();
        return result > 0;
    }
}
