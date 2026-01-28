using Commerce.Api.Storage;
using Commerce.IntegrationTests.Fakes;
using Commerce.Repositories.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Commerce.IntegrationTests;

public sealed class TestAppFactory : WebApplicationFactory<Program>
{
    private readonly PostgresContainerFixture _postgres;

    /// <summary>
    /// Shared fake storage service instance for assertions in tests.
    /// </summary>
    public FakeObjectStorageService FakeStorage { get; } = new();

    public TestAppFactory(PostgresContainerFixture postgres)
    {
        _postgres = postgres;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.ConnectionString
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContextOptions registration if present
            services.RemoveAll(typeof(DbContextOptions<CommerceDbContext>));

            services.AddDbContext<CommerceDbContext>(o =>
                o.UseNpgsql(_postgres.ConnectionString));

            // Replace real storage service with fake for tests
            services.RemoveAll<IObjectStorageService>();
            services.AddSingleton<IObjectStorageService>(FakeStorage);
        });

        var host = base.CreateHost(builder);

        // Apply migrations into the fresh container DB
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CommerceDbContext>();
        db.Database.Migrate();

        return host;
    }
}
