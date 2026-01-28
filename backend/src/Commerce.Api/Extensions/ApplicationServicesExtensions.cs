using System.Diagnostics.CodeAnalysis;
using Commerce.Api.Validation;
using Commerce.Repositories;
using Commerce.Services;
using Commerce.Shared.Requests;
using FluentValidation;

namespace Commerce.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        #region Services Registration
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        #endregion
        #region Repositories Registration
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        #endregion
        #region Validators Registration
        services.AddValidatorsFromAssemblyContaining<GetProductsQueryParams>();
        services.AddScoped<FluentValidationActionFilter>();
        #endregion
        return services;
    }
}
