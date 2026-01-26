using System.Diagnostics.CodeAnalysis;
using Commerce.Api.Extensions;
using Commerce.Api.Mappers;
using Commerce.Services;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>
    /// Gets all categories filtered by query parameters.
    /// Admin can pass IsActive=null to get active+inactive.
    /// </summary>
    [HttpGet("")]
    [ProducesResponseType(typeof(List<CategoryResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQueryParams queryParams, CancellationToken ct)
    {
        var page = await categoryService.GetCategoriesAsync(queryParams, ct);

        HttpContext.SetPaging(page);

        return page.Items.Count > 0 ? Ok(page.Items) : NoContent();
    }

    /// <summary>
    /// Get admin category details (includes parents/children Id+Name).
    /// </summary>
    [HttpGet("{categoryId:int}")]
    [ProducesResponseType(typeof(CategoryAdminDetailsResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryById(int categoryId, CancellationToken ct)
    {
        var details = await categoryService.GetCategoryAdminDetailsAsync(categoryId, ct);
        return details is not null ? Ok(details) : NotFound();
    }

    /// <summary>
    /// Get root categories for navigation (active-only by default).
    /// </summary>
    [HttpGet("roots")]
    [ProducesResponseType(typeof(List<CategoryResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetRoots([FromQuery] bool includeInactive = false, CancellationToken ct = default)
    {
        var roots = await categoryService.GetRootsAsync(includeInactive, ct);
        return roots.Count > 0 ? Ok(roots) : NoContent();
    }

    /// <summary>
    /// Get children categories for a given parent (active-only by default).
    /// </summary>
    [HttpGet("{parentCategoryId:int}/children")]
    [ProducesResponseType(typeof(List<CategoryResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetChildren(int parentCategoryId, [FromQuery] bool includeInactive = false, CancellationToken ct = default)
    {
        var children = await categoryService.GetChildrenAsync(parentCategoryId, includeInactive, ct);
        return children.Count > 0 ? Ok(children) : NoContent();
    }

    /// <summary>
    /// Adds a new category (reuses existing by name) and optionally attaches it under parents.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest categoryRequest, CancellationToken ct)
    {
        var (result, categoryId) = await categoryService.AddCategoryAsync(categoryRequest, ct);

        return this.ToActionResult(
            result,
            onSuccess: () => CreatedAtAction(nameof(GetCategoryById), new { categoryId }, null)
        );
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCategory([FromBody] CreateCategoryRequest categoryRequest, int categoryId, CancellationToken ct)
    {
        var result = await categoryService.UpdateCategoryAsync(categoryRequest, categoryId, ct);
        return this.ToActionResult(result, onSuccess: NoContent);
    }

    /// <summary>
    /// Toggle the active status of a category.
    /// </summary>
    [HttpPatch("toggle/{categoryId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ToggleCategory(int categoryId, CancellationToken ct)
    {
        var result = await categoryService.ToggleCategoryAsync(categoryId, ct);
        return this.ToActionResult(result, onSuccess: NoContent);
    }

    /// <summary>
    /// Attach a child category to a parent category.
    /// </summary>
    [HttpPost("{parentCategoryId:int}/children/{childCategoryId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AttachChild(int parentCategoryId, int childCategoryId, CancellationToken ct)
    {
        var result = await categoryService.AttachCategoryAsync(parentCategoryId, childCategoryId, ct);
        return this.ToActionResult(result, onSuccess: NoContent);
    }

    /// <summary>
    /// Detach a child category from a parent category.
    /// </summary>
    [HttpDelete("{parentCategoryId:int}/children/{childCategoryId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DetachChild(int parentCategoryId, int childCategoryId, CancellationToken ct)
    {
        var result = await categoryService.DetachCategoryAsync(parentCategoryId, childCategoryId, ct);
        return this.ToActionResult(result, onSuccess: NoContent);
    }
}
