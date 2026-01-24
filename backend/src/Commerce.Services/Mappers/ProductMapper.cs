using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product) =>
        new()
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive
        };
}
