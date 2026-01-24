using Commerce.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Api.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CommerceDbContext>(options =>
        {
            var cs = config.GetConnectionString("DefaultConnection");
            options.UseNpgsql(cs);
        });

        return services;
    }
}
