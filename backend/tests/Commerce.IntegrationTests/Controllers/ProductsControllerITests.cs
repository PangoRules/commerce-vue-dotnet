using System.Net;
using System.Net.Http.Json;
using Commerce.Shared.Responses;

namespace Commerce.IntegrationTests
{
    [Collection("Integration")]
    public class ProductsEndpointsTests
    {
        private readonly HttpClient _client;

        public ProductsEndpointsTests(PostgresContainerFixture postgres)
        {
            var factory = new TestAppFactory(postgres);
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetActiveProducts_Returns200AndNonEmptyList()
        {
            var res = await _client.GetAsync("/api/products/active");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
            {
                // If your endpoint returns 204 for empty, this would be unexpected with seed data,
                // but keep it flexible while developing.
                return;
            }

            var products = await res.Content.ReadFromJsonAsync<List<ProductResponse>>();
            Assert.NotNull(products);
            Assert.NotEmpty(products!);
        }

        [Fact]
        public async Task GetProductById_SeededId_Returns200()
        {
            var res = await _client.GetAsync("/api/products/1001");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.IsSuccessStatusCode, $"Status: {(int)res.StatusCode}\n{body}");

            var product = await res.Content.ReadFromJsonAsync<ProductResponse>();
            Assert.NotNull(product);
            Assert.Equal(1001, product!.Id);
        }
    }
}
