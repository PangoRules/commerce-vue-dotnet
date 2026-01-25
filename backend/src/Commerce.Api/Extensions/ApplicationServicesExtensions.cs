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
        services.AddScoped<IProductsServices, ProductsServices>();
        #endregion
        #region Repositories Registration
        services.AddScoped<IProductsRepository, ProductsRepository>();
        #endregion
        #region Validators Registration
        services.AddValidatorsFromAssemblyContaining<GetProductsQueryParams>();
        services.AddScoped<FluentValidationActionFilter>();
        #endregion
        return services;
    }
}
