using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Repositories.Context;

[ExcludeFromCodeCoverage]
public class CommerceDbContext(DbContextOptions<CommerceDbContext> options) : DbContext(options)
{
    //DBSets go here later

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations if you add them later:
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommerceDbContext).Assembly);
    }
}
