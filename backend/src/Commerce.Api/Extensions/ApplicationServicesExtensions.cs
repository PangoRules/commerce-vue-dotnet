namespace Commerce.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services / repos
        services.AddScoped<Commerce.Services.IHealthService, Commerce.Services.HealthService>();

        return services;
    }
}
