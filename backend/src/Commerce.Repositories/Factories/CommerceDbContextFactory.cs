using Commerce.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Commerce.Repositories.Factories
{
    public class CommerceDbContextFactory : IDesignTimeDbContextFactory<CommerceDbContext>
    {
        public CommerceDbContext CreateDbContext(string[] args)
        {
            var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException(
                    "Connection string 'ConnectionStrings__DefaultConnection' is not configured.");
            }


            var options = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseNpgsql(cs)
                .Options;

            return new CommerceDbContext(options);
        }
    }
}
