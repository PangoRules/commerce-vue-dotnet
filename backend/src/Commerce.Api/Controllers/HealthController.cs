using Commerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint for service availability.
    /// </summary>
    /// <remarks>
    /// Used by Docker, load balancers, and monitoring systems to verify
    /// that the API is running and able to respond to requests.
    /// </remarks>
    /// <response code="200">Service is healthy</response>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get() => new OkObjectResult(new HealthResponse
    {
        Status = "Healthy",
        Timestamp = DateTime.UtcNow
    });
}
