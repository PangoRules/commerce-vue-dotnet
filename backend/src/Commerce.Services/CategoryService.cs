using Commerce.Repositories;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.Services;

public interface ICategoryService
{
    /// <summary>
    /// Retrieve a paginated list of categories based on query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter categories.</param>
    /// <returns>A paged result of category responses.</returns>
    Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQueryParams queryParams);
    
    /// <summary>
    /// Add a new category.
    /// </summary>
    /// <param name="categoryRequest">The request object containing category details.</param>
    /// <returns>True if the category was added successfully, otherwise false.</returns>
    Task<bool> AddCategoryAsync(CreateCategoryRequest categoryRequest);
    
    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="categoryRequest">The request object containing updated category details.</param>
    /// <param name="categoryId">The ID of the category to update.</param>
    /// <returns>True if the category was updated successfully, otherwise false.</returns>
    Task<bool> UpdateCategoryAsync(CreateCategoryRequest categoryRequest, int categoryId);
    
    /// <summary>
    /// Toggle the active status of a category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to toggle.</param>
    /// <returns>True if the category was toggled successfully, otherwise false.</returns>
    Task<bool> ToggleCategoryAsync(int categoryId);
}

public class CategoryService(ICategoriesRepository categoriesRepository) : ICategoryService
{
    public async Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQueryParams queryParams)
    {
        var categoriesPagedResult = await categoriesRepository.GetAllCategoriesAsync(queryParams);
        return new PagedResult<CategoryResponse>(
            [.. categoriesPagedResult.Items.Select(Mappers.CategoryMapper.ToResponse)],
            categoriesPagedResult.TotalCount,
            categoriesPagedResult.Page,
            categoriesPagedResult.PageSize);
    }

    public Task<bool> AddCategoryAsync(CreateCategoryRequest categoryRequest)
    {
        return categoriesRepository.AddCategoryAsync(categoryRequest);
    }

    public Task<bool> UpdateCategoryAsync(CreateCategoryRequest categoryRequest, int categoryId)
    {
        return categoriesRepository.UpdateCategoryAsync(categoryRequest, categoryId);
    }

    public Task<bool> ToggleCategoryAsync(int categoryId)
    {
        return categoriesRepository.ToggleCategoryAsync(categoryId);
    }
}