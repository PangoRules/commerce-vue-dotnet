using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Commerce.Api.Validation;

namespace Commerce.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ApiControllersExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers( opts =>
            {
                opts.Filters.Add<PagingHeadersFilter>();
                opts.Filters.AddService<FluentValidationActionFilter>();
            })
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
