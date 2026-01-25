using System.Diagnostics.CodeAnalysis;
using Commerce.Api.Extensions;
using Commerce.Services;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>
    /// Gets all categories filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter categories.</param>
    /// <returns>A list of category responses.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(List<CategoryResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQueryParams queryParams)
    {
        var page =  await categoryService.GetCategoriesAsync(queryParams);

        HttpContext.SetPaging(page);

        return page.Items.Count > 0 ? Ok(page.Items) : NoContent();
    }

    /// <summary>
    /// Adds a new category.
    /// </summary>
    /// <param name="categoryRequest">The request object containing category details.</param>
    /// <returns>No content if successful, otherwise a bad request response.</returns>
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest categoryRequest)
    {
        var result = await categoryService.AddCategoryAsync(categoryRequest);
        if (!result)
        {
            return BadRequest("Failed to add category.");
        }
        return NoContent();
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="categoryRequest">The request object containing updated category details.</param>
    /// <param name="categoryId">The ID of the category to update.</param>
    /// <returns>No content if successful, otherwise a bad request response.</returns>
    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateCategory([FromBody] CreateCategoryRequest categoryRequest, int categoryId)
    {
        var result = await categoryService.UpdateCategoryAsync(categoryRequest, categoryId);
        if (!result)
        {
            return BadRequest("Failed to update category.");
        }
        return NoContent();
    }

    /// <summary>
    /// Toggle the active status of a category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to toggle.</param>
    /// <returns>No content if successful, otherwise a bad request response.</returns>
    [HttpPatch("toggle/{categoryId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ToggleCategory(int categoryId)
    {
        var result = await categoryService.ToggleCategoryAsync(categoryId);
        if (!result)
        {
            return BadRequest("Failed to toggle category.");
        }
        return NoContent();
    }
}