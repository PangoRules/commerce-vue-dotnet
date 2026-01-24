using Commerce.Repositories.Context;

namespace Commerce.Services;

public interface IHealthService
{
    /// <summary>
    /// Checks database connectivity.
    /// </summary>
    /// <param name="ct">cancellation token</param>
    /// <returns>a tuple indicating if the database is healthy and a message describing the result</returns>
    Task<(bool ok, string message)> CheckDbAsync(CancellationToken ct);
}

/// <summary>
/// Service for health checks.
/// </summary>
/// <param name="db">The database context to use for health checks.</param>
public class HealthService(CommerceDbContext db) : IHealthService
{
    public async Task<(bool ok, string message)> CheckDbAsync(CancellationToken ct)
    {
        try
        {
            var canConnect = await db.Database.CanConnectAsync(ct);
            return canConnect ? (true, "db ok") : (false, "db cannot connect");
        }
        catch (Exception ex)
        {
            return (false, $"db error: {ex.Message}");
        }
    }
}
