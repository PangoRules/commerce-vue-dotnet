using System.Diagnostics.CodeAnalysis;
using Commerce.Services;
using Commerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

/// <summary>
/// Controller for health check endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public class HealthController(IHealthService healthService) : ControllerBase
{
    /// <summary>
    /// Health check endpoint for service availability. Also checks db for completeness.
    /// </summary>
    /// <remarks>
    /// Used by Docker, load balancers, and monitoring systems to verify
    /// that the API is running and able to respond to requests.
    /// </remarks>
    /// <response code="200">Service is healthy</response>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        CancellationToken ct)
    {
        var (ok, message) = await healthService.CheckDbAsync(ct);

        return Ok(new HealthResponse
        {
            Status = ok ? HealthStatus.Healthy : HealthStatus.Unhealthy,
            Timestamp = DateTime.UtcNow,
            Db = new DbHealthResponse
            {
                Status = ok ? HealthStatus.Healthy : HealthStatus.Unhealthy,
                Message = message
            }
        });
    }

    /// <summary>
    /// Health check endpoint for database connectivity.
    /// </summary>
    /// <param name="ct">The cancellation token to cancel the operation.</param>
    /// <returns code="200">Database is healthy</returns>
    /// <returns code="500">Database is unhealthy</returns>
    [HttpGet("db")]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetDbAsync(CancellationToken ct)
    {
        var (ok, message) = await healthService.CheckDbAsync(ct);

        if (ok)
        {
            return Ok(new HealthResponse
            {
                Status = HealthStatus.Healthy,
                Timestamp = DateTime.UtcNow
            });
        }

        return Problem(
            title: "Database connectivity failure",
            detail: message,
            statusCode: StatusCodes.Status503ServiceUnavailable,
            type: "https://httpstatuses.com/503"
        );
    }
}