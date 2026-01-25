using System.Net;

namespace Commerce.IntegrationTests.Controllers;

[Collection("Integration")]
public class HealthEndpointsITests
{
    private readonly HttpClient _client;

    public HealthEndpointsITests(PostgresContainerFixture postgres)
        {
            var factory = new TestAppFactory(postgres);
            _client = factory.CreateClient();
        }

    [Fact]
    public async Task Health_Returns200()
    {
        var res = await _client.GetAsync("/api/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
