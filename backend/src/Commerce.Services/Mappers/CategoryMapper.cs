using Commerce.Repositories.Entities;
using Commerce.Shared.Responses;

namespace Commerce.Services.Mappers;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(Category category) =>
        new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
}
