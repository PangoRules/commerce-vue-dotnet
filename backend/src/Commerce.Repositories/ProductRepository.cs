using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories.Context;
using Commerce.Repositories.Entities;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories;

public interface IProductRepository
{
    /// <summary>
    /// Get product by its ID.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The product if found, otherwise null.</returns>
    Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default);

    /// <summary>
    /// Get all products filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter products.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paged result of products.</returns>
    Task<PagedResult<Product>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct = default);

    /// <summary>
    /// Add a new product.
    /// </summary>
    /// <param name="product">The product request to add.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation and the new product ID.</returns>
    Task<(DbResultOption Result, int ProductId)> AddProductAsync(CreateProductRequest product, CancellationToken ct = default);

    /// <summary>
    /// Update an existing product.
    /// </summary>
    /// <param name="product">The product request to update.</param>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation and the updated product if successful, otherwise null.</returns>
    Task<(DbResultOption Result, Product? Product)> UpdateProductAsync(UpdateProductRequest product, int productId, CancellationToken ct = default);

    /// <summary>
    /// Toggle the active status of a product.
    /// </summary>
    /// <param name="productId">The ID of the product to toggle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> ToggleProductAsync(int productId, CancellationToken ct = default);
}

[ExcludeFromCodeCoverage]
public class ProductRepository(CommerceDbContext context) : IProductRepository
{
    public Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
    {
        return context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);
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

    public async Task<(DbResultOption Result, int ProductId)> AddProductAsync(CreateProductRequest product, CancellationToken ct = default)
    {
        var entity = Product.FromCreateRequest(product);

        try
        {
            await context.Products.AddAsync(entity, ct);
            await context.SaveChangesAsync(ct);
            return (DbResultOption.Success, entity.Id);
        }
        catch (DbUpdateException)
        {
            // FK violation (bad CategoryId) or unique constraint, etc.
            return (DbResultOption.Conflict, 0);
        }
    }

    public async Task<(DbResultOption Result, Product? Product)> UpdateProductAsync(UpdateProductRequest product, int productId, CancellationToken ct = default)
    {
        var existing = await context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (existing is null) return (DbResultOption.NotFound, null);

        existing.UpdateProduct(product);

        try
        {
            var changed = await context.SaveChangesAsync(ct);
            return changed > 0
                ? (DbResultOption.Success, existing)
                : (DbResultOption.Error, null);
        }
        catch (DbUpdateException)
        {
            return (DbResultOption.Conflict, null);
        }
    }

    public async Task<DbResultOption> ToggleProductAsync(int productId, CancellationToken ct = default)
    {
        var existing = await context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (existing is null) return DbResultOption.NotFound;

        existing.ToggleProduct();

        try
        {
            return await context.SaveChangesAsync(ct) > 0 ? DbResultOption.Success : DbResultOption.Error;
        }
        catch (DbUpdateException)
        {
            return DbResultOption.Conflict;
        }
    }
}
