using Commerce.Repositories.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Commerce.IntegrationTests;

public sealed class TestAppFactory(PostgresContainerFixture postgres) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = postgres.ConnectionString
            };

            config.AddInMemoryCollection(settings);
        });

        // Optional but recommended: ensure DbContext uses this connection string even if AddPersistence changes
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContextOptions registration if present
            services.RemoveAll(typeof(DbContextOptions<CommerceDbContext>));

            services.AddDbContext<CommerceDbContext>(o =>
                o.UseNpgsql(postgres.ConnectionString));
        });

        var host = base.CreateHost(builder);

        // Apply migrations into the fresh container DB
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CommerceDbContext>();
        db.Database.Migrate();

        return host;
    }
}
