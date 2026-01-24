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
public class ProductsController(IProductsServices productsServices) : ControllerBase
{
    /// <summary>
    /// Gets a product by its identifier.
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
    /// Gets all active products by category identifier.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category to filter products.</param>
    /// <returns>A list of active product responses in the specified category.</returns>
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(List<ProductResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetAllActiveProductsByCategoryId(int categoryId)
    {
        var products = await productsServices.GetAllActiveProductsByCategoryIdAsync(categoryId);
        return products.Count > 0 ? Ok(products) : NoContent();
    }

    /// <summary>
    /// Gets all active products.
    /// </summary>
    /// <returns>A list of active product responses.</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<ProductResponse>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetAllActiveProducts()
    {
        var products = await productsServices.GetAllActiveProductsAsync();
        return products.Count > 0 ? Ok(products) : NoContent();
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