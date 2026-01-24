using System.Text.Json.Serialization;

namespace Commerce.Api.Extensions;

public static class ApiControllersExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static WebApplication MapApiControllers(this WebApplication app)
    {
        app.MapControllers();
        return app;
    }
}
