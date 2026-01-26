using System.Diagnostics.CodeAnalysis;
using Commerce.Api.Extensions;
using Commerce.Services;
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
public class ProductController(IProductsService productsServices) : ControllerBase
{
    /// <summary>
    /// Gets a product by its identifier (with category information).
    /// </summary>
    /// <param name="productId">The unique identifier of the product to retrieve.</param>
    /// <returns>The product response if found, otherwise a 404 Not Found response.</returns>
    [HttpGet("{productId:int}")]
    [ProducesResponseType(typeof(ProductResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProductById(int productId)
    {
        var product = await productsServices.GetProductByIdAsync(productId);
        if (product is null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Gets all products filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter products.</param>
    /// <returns>A list of product responses.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(List<ProductResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetProductsQueryParams queryParams)
{
    var page = await productsServices.GetAllProductsAsync(queryParams);

    HttpContext.SetPaging(page);

    return page.Items.Count > 0 ? Ok(page.Items) : NoContent();
}


    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product data to be added.</param>
    /// <returns>No content if successful, otherwise a 400 Bad Request response.</returns>
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest product)
    {
        var result = await productsServices.AddProductAsync(product);
        if (!result)
        {
            return BadRequest("Failed to add product.");
        }
        return NoContent();
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to update.</param>
    /// <param name="product">The updated product data.</param>
    /// <returns>The updated product response if successful, otherwise a 404 Not Found response.</returns>
    [HttpPut("{productId:int}")]
    [ProducesResponseType(typeof(ProductResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProductRequest product)
    {
        var updatedProduct = await productsServices.UpdateProductAsync(product, productId);
        if (updatedProduct is null)
        {
            return NotFound();
        }
        return Ok(updatedProduct);
    }

    /// <summary>
    /// Toggles the active status of a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to toggle.</param>
    /// <returns>No content if successful, otherwise a 404 Not Found response.</returns>
    [HttpPatch("toggle/{productId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ToggleProduct(int productId)
    {
        var result = await productsServices.ToggleProductAsync(productId);
        if (!result)
        {
            return NotFound();
        }
        return Ok();
    }
}