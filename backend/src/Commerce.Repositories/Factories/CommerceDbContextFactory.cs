using Commerce.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Commerce.Repositories.Factories
{
    public class CommerceDbContextFactory : IDesignTimeDbContextFactory<CommerceDbContext>
    {
        public CommerceDbContext CreateDbContext(string[] args)
        {
            // Used ONLY by dotnet-ef at design time.
            // Keep it simple and predictable.
            var cs =
                Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
                ?? "Host=localhost;Port=5432;Database=commerce;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseNpgsql(cs)
                .Options;

            return new CommerceDbContext(options);
        }
    }
}
