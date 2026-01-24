namespace Commerce.Api.Extensions;

public static class HttpsExtensions
{
    public static WebApplication UseApiHttps(this WebApplication app)
    {
        // Keep as-is for now (you said you like it).
        // If Docker HTTP-only ever annoys you, guard with: if (!app.Environment.IsDevelopment()) ...
        app.UseHttpsRedirection();
        return app;
    }
}
