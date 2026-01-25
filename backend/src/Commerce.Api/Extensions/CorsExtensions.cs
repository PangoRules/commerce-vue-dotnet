using System.Diagnostics.CodeAnalysis;

namespace Commerce.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class CorsExtensions
{
    public const string FrontendDevPolicy = "FrontendDev";

    public static IServiceCollection AddApiCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(FrontendDevPolicy, policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static WebApplication UseApiCors(this WebApplication app)
    {
        app.UseCors(FrontendDevPolicy);
        return app;
    }
}
