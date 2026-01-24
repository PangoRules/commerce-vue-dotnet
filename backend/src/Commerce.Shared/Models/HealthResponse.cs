namespace Commerce.Shared.Models;

public class HealthResponse
{
    /// <summary>
    /// Status of the service.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the health check response.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
