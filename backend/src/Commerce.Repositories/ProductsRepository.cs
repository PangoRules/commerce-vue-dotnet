using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
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
    /// Get all products filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter products.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paged result of products.</returns>
    public Task<PagedResult<Product>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct = default);

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

[ExcludeFromCodeCoverage]
public class ProductsRepository(CommerceDbContext context) : IProductsRepository
{
    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await context.Products.FindAsync(productId);
    }

    public async Task<PagedResult<Product>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct)
    {
        var query = context.Products.AsNoTracking().AsQueryable();

        if (queryParams.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == queryParams.CategoryId.Value);
        }

        if (queryParams.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == queryParams.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var term = queryParams.SearchTerm.Trim();

            // SQL LIKE pattern
            var pattern = $"%{term}%";

            query = query.Where(p =>
                EF.Functions.Like(p.Name, pattern) ||
                (p.Description != null && EF.Functions.Like(p.Description, pattern)));
        }

        // Total count AFTER filters, BEFORE pagination
        var totalCount = await query.CountAsync(ct);

        // Stable ordering (always deterministic)
        query = ApplyOrdering(query, queryParams);

        // Pagination
        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Product>(items, queryParams.Page, queryParams.PageSize, totalCount);
    }

    private static IQueryable<Product> ApplyOrdering(IQueryable<Product> query, GetProductsQueryParams q)
    {
        // Primary sort
        query = (q.SortBy, q.SortDescending) switch
        {
            (ProductSortBy.Name, false) => query.OrderBy(p => p.Name),
            (ProductSortBy.Name, true) => query.OrderByDescending(p => p.Name),

            (ProductSortBy.Price, false) => query.OrderBy(p => p.Price),
            (ProductSortBy.Price, true) => query.OrderByDescending(p => p.Price),

            (ProductSortBy.StockQuantity, false) => query.OrderBy(p => p.StockQuantity),
            (ProductSortBy.StockQuantity, true) => query.OrderByDescending(p => p.StockQuantity),

            (ProductSortBy.CategoryId, false) => query.OrderBy(p => p.CategoryId),
            (ProductSortBy.CategoryId, true) => query.OrderByDescending(p => p.CategoryId),

            _ => query.OrderBy(p => p.Name)
        };

        return ((IOrderedQueryable<Product>)query).ThenBy(p => p.Id);
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
