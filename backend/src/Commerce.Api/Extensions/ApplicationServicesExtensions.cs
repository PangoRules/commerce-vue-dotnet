using System.Diagnostics.CodeAnalysis;
using Commerce.Repositories;
using Commerce.Services;

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
        return services;
    }
}
