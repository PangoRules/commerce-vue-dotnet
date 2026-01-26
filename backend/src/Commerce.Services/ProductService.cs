using Commerce.Repositories;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.Services;

public interface IProductService
{
    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to retrieve.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The product response if found, otherwise null.</returns>
    public Task<ProductResponse?> GetProductByIdAsync(int productId, CancellationToken ct = default);

    /// <summary>
    /// Gets all products filtered by query parameters.
    /// </summary>
    /// <param name="queryParams">The query parameters to filter products.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A paged result of product responses.</returns>
    public Task<PagedResult<ProductResponse>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct = default);

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product request data to create a new product.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation and the new product ID if successful, otherwise an error result.</returns>
    Task<(DbResultOption Result, int ProductId)> AddProductAsync(CreateProductRequest product, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product request data to update the product.</param>
    /// <param name="productId">The unique identifier of the product to update.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The updated product response if successful, otherwise null.</returns>
    Task<(DbResultOption Result, ProductResponse? Product)> UpdateProductAsync(UpdateProductRequest product, int productId, CancellationToken ct = default);

    /// <summary>
    /// Toggles the active status of a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to toggle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<DbResultOption> ToggleProductAsync(int productId, CancellationToken ct = default);
}

public class ProductService(
    IProductRepository productsRepo,
    ICategoryRepository categoriesRepo
) : IProductService
{
    public async Task<ProductResponse?> GetProductByIdAsync(int productId, CancellationToken ct = default)
    {
        var product = await productsRepo.GetProductByIdAsync(productId, ct);
        return product is null ? null : Mappers.ProductMapper.ToResponse(product);
    }

    public async Task<PagedResult<ProductResponse>> GetAllProductsAsync(GetProductsQueryParams queryParams, CancellationToken ct = default)
    {
        var paged = await productsRepo.GetAllProductsAsync(queryParams, ct);

        return new PagedResult<ProductResponse>(
            [.. paged.Items.Select(Mappers.ProductMapper.ToResponse)],
            paged.Page,
            paged.PageSize,
            paged.TotalCount
        );
    }

    public async Task<(DbResultOption Result, int ProductId)> AddProductAsync(CreateProductRequest product, CancellationToken ct = default)
    {
        var category = await categoriesRepo.GetByIdAsync(product.CategoryId, ct);
        if (category is null) return (DbResultOption.NotFound, 0);
        if (!category.IsActive) return (DbResultOption.Invalid, 0);

        return await productsRepo.AddProductAsync(product, ct);
    }

    public async Task<(DbResultOption Result, ProductResponse? Product)> UpdateProductAsync(UpdateProductRequest product, int productId, CancellationToken ct = default)
    {
        var category = await categoriesRepo.GetByIdAsync(product.CategoryId, ct);
        if (category is null) return (DbResultOption.NotFound, null);
        if (!category.IsActive) return (DbResultOption.Invalid, null);

        var (result, updatedEntity) = await productsRepo.UpdateProductAsync(product, productId, ct);

        return result == DbResultOption.Success && updatedEntity is not null
            ? (DbResultOption.Success, Mappers.ProductMapper.ToResponse(updatedEntity))
            : (result, null);
    }

    public Task<DbResultOption> ToggleProductAsync(int productId, CancellationToken ct = default)
        => productsRepo.ToggleProductAsync(productId, ct);
}