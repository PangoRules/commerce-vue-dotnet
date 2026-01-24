namespace Commerce.Api.Extensions;

public static class ApiControllersExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }

    public static WebApplication MapApiControllers(this WebApplication app)
    {
        app.MapControllers();
        return app;
    }
}
