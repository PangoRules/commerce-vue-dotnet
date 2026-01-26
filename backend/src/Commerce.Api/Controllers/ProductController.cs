using System.Diagnostics.CodeAnalysis;
using Commerce.Api.Extensions;
using Commerce.Api.Mappers;
using Commerce.Services;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

/// <summary>
/// Controller for product-related endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class ProductController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Gets a product by its identifier (with category information).
    /// </summary>
    [HttpGet("{productId:int}")]
    [ProducesResponseType(typeof(ProductResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProductById(int productId, CancellationToken ct)
    {
        var product = await productService.GetProductByIdAsync(productId, ct);
        return product is not null ? Ok(product) : NotFound();
    }

    /// <summary>
    /// Gets all products filtered by query parameters.
    /// </summary>
    [HttpGet("")]
    [ProducesResponseType(typeof(List<ProductResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetProductsQueryParams queryParams, CancellationToken ct)
    {
        var page = await productService.GetAllProductsAsync(queryParams, ct);

        HttpContext.SetPaging(page);

        return page.Items.Count > 0 ? Ok(page.Items) : NoContent();
    }

    /// <summary>
    /// Adds a new product.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest product, CancellationToken ct)
    {
        var (result, productId) = await productService.AddProductAsync(product, ct);

        return this.ToActionResult(
            result,
            onSuccess: () => CreatedAtAction(nameof(GetProductById), new { productId }, null)
        );
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{productId:int}")]
    [ProducesResponseType(typeof(ProductResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProductRequest product, CancellationToken ct)
    {
        var (result, updated) = await productService.UpdateProductAsync(product, productId, ct);

        if (result == DbResultOption.Success && updated is null)
            return StatusCode(500, "Update succeeded but no product was returned.");

        return this.ToActionResult(
            result,
            onSuccess: () => Ok(updated)
        );
    }

    /// <summary>
    /// Toggles the active status of a product.
    /// </summary>
    [HttpPatch("toggle/{productId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ToggleProduct(int productId, CancellationToken ct)
    {
        var result = await productService.ToggleProductAsync(productId, ct);
        return this.ToActionResult(result, onSuccess: NoContent);
    }
}
