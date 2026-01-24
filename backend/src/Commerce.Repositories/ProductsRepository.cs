using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Requests;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;
public interface IProductsRepository
{
    /// <summary>
    /// Get product by its ID.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <returns>The product if found, otherwise null.</returns>
    public Task<Product?> GetProductByIdAsync(int productId);

    /// <summary>
    /// Get all active products by category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve products for.</param>
    /// <returns>A list of active products in the specified category.</returns>
    public Task<List<Product>> GetAllActiveProductsByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Get all active products.
    /// </summary>
    /// <returns>A list of all active products.</returns>
    public Task<List<Product>> GetAllActiveProductsAsync();

    /// <summary>
    /// Add a new product.
    /// </summary>
    /// <param name="product">The product request to add.</param>
    /// <returns>True if the product was added successfully, otherwise false.</returns>
    public Task<bool> AddProductAsync(CreateProductRequest product);

    /// <summary>
    /// Update an existing product.
    /// </summary>
    /// <param name="product">The product request to update.</param>
    /// <param name="productId">The ID of the product to update.</param>
    /// <returns>The updated product if successful, otherwise null.</returns>
    public Task<Product?> UpdateProductAsync(UpdateProductRequest product, int productId);

    /// <summary>
    /// Toggle the active status of a product.
    /// </summary>
    /// <param name="productId">The ID of the product to toggle.</param>
    /// <returns>True if the product was toggled successfully, otherwise false.</returns>
    public Task<bool> ToggleProductAsync(int productId);
}

public class ProductsRepository(CommerceDbContext context) : IProductsRepository
{
    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await context.Products.FindAsync(productId);
    }

    public async Task<List<Product>> GetAllActiveProductsByCategoryIdAsync(int categoryId)
    {
        return await context.Products.AsNoTracking().Where(p => p.CategoryId == categoryId && p.IsActive).ToListAsync();
    }

    public async Task<List<Product>> GetAllActiveProductsAsync()
    {
        return await context.Products.AsNoTracking().Where(p => p.IsActive).ToListAsync();
    }

    public async Task<bool> AddProductAsync(CreateProductRequest product)
    {
        var entity = Product.FromCreateRequest(product);
        await context.Products.AddAsync(entity);
        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<Product?> UpdateProductAsync(UpdateProductRequest product, int productId)
    {
        var existingProduct = await context.Products.FindAsync(productId);
        if (existingProduct == null)
        {
            return null;
        }

        existingProduct.UpdateProduct(product);
        var result = await context.SaveChangesAsync();
        return result > 0 ? existingProduct : null;
    }

    public async Task<bool> ToggleProductAsync(int productId)
    {
        var existingProduct = await context.Products.FindAsync(productId);
        if (existingProduct == null)
        {
            return false;
        }

        existingProduct.ToggleProduct();
        var result = await context.SaveChangesAsync();
        return result > 0;
    }
}
