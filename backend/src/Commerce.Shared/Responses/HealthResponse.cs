using System.Diagnostics.CodeAnalysis;

namespace Commerce.Shared.Responses;


[ExcludeFromCodeCoverage]
public class HealthResponse
{
    /// <summary>
    /// Status of the service.
    /// </summary>
    public HealthStatus Status { get; set; } = default!;

    /// <summary>
    /// Timestamp of the health check response.
    /// </summary>
    public DateTime Timestamp { get; set; }

    public DbHealthResponse? Db { get; set; }
}


[ExcludeFromCodeCoverage]
public class DbHealthResponse
{
    public HealthStatus Status { get; set; } = default!;   // "Healthy" / "Unhealthy"
    public string Message { get; set; } = default!;  // "db ok" / error detail
}

public enum HealthStatus
{
    Healthy,
    Unhealthy,
    Degraded
}