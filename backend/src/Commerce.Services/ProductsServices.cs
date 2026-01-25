using Commerce.Repositories;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.Services;

public interface IProductsServices
{
    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to retrieve.</param>
    /// <returns>The product response if found, otherwise null.</returns>
    public Task<ProductResponse?> GetProductByIdAsync(int productId);

    /// <summary>
    /// Gets all active products by category identifier.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category to filter products.</param>
    /// <returns>A list of active product responses in the specified category.</returns>
    public Task<List<ProductResponse>> GetAllActiveProductsByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Gets all active products.
    /// </summary>
    /// <returns>A list of all active product responses.</returns>
    public Task<List<ProductResponse>> GetAllActiveProductsAsync();

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product request data to create a new product.</param>
    /// <returns>True if the product was successfully added, otherwise false.</returns>
    public Task<bool> AddProductAsync(CreateProductRequest product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product request data to update the product.</param>
    /// <param name="productId">The unique identifier of the product to update.</param>
    /// <returns>The updated product response if successful, otherwise null.</returns>
    public Task<ProductResponse?> UpdateProductAsync(UpdateProductRequest product, int productId);

    /// <summary>
    /// Toggles the active status of a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to toggle.</param>
    /// <returns>True if the product was successfully toggled, otherwise false.</returns>
    public Task<bool> ToggleProductAsync(int productId);
}

public class ProductsServices(IProductsRepository repository) : IProductsServices
{
    public async Task<ProductResponse?> GetProductByIdAsync(int productId)
    {
        var product =  await repository.GetProductByIdAsync(productId);
        if (product is null)
        {
            return null;
        }
        return Mappers.ProductMapper.ToResponse(product);
    }

    public async Task<List<ProductResponse>> GetAllActiveProductsByCategoryIdAsync(int categoryId)
    {
        var products = await repository.GetAllActiveProductsByCategoryIdAsync(categoryId);
        return [.. products.Select(Mappers.ProductMapper.ToResponse)];
    }

    public async Task<List<ProductResponse>> GetAllActiveProductsAsync()
    {
        var products = await repository.GetAllActiveProductsAsync();
        return [.. products.Select(Mappers.ProductMapper.ToResponse)];
    }

    public async Task<bool> AddProductAsync(CreateProductRequest product)
    {
        return await repository.AddProductAsync(product);
    }

    public async Task<ProductResponse?> UpdateProductAsync(UpdateProductRequest product, int productId)
    {
        var updatedProduct = await repository.UpdateProductAsync(product, productId);
        return updatedProduct is not null ? Mappers.ProductMapper.ToResponse(updatedProduct) : null;
    }

    public async Task<bool> ToggleProductAsync(int productId)
    {
        return await repository.ToggleProductAsync(productId);
    }
}