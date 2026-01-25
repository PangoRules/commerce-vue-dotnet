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
        public async Task GetAllProducts_NoParams_Returns200Or204_AndListIf200()
        {
            var res = await _client.GetAsync("/api/products");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
                return;

            var products = await res.Content.ReadFromJsonAsync<List<ProductResponse>>();
            Assert.NotNull(products);
            // If you have seed data, keep NotEmpty; otherwise remove it.
            Assert.NotEmpty(products!);
        }

        [Fact]
        public async Task GetAllProducts_WithPagination_ReturnsPagedItems_AndPagingHeaders()
        {
            var res = await _client.GetAsync("/api/products?page=1&pageSize=2");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
                return;

            var items = await res.Content.ReadFromJsonAsync<List<ProductResponse>>();
            Assert.NotNull(items);

            Assert.InRange(items!.Count, 1, 2);

            // If you add paging headers universally, validate them here:
            Assert.True(res.Headers.Contains("X-Total-Count"));
            Assert.True(res.Headers.Contains("X-Page"));
            Assert.True(res.Headers.Contains("X-Page-Size"));

            var totalCount = int.Parse(res.Headers.GetValues("X-Total-Count").Single());
            var page = int.Parse(res.Headers.GetValues("X-Page").Single());
            var pageSize = int.Parse(res.Headers.GetValues("X-Page-Size").Single());

            Assert.Equal(1, page);
            Assert.Equal(2, pageSize);
            Assert.True(totalCount >= items.Count);
        }

        [Fact]
        public async Task GetAllProducts_FilterIsActiveTrue_ReturnsOnlyActive()
        {
            var res = await _client.GetAsync("/api/products?isActive=true");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
                return;

            var items = await res.Content.ReadFromJsonAsync<List<ProductResponse>>();
            Assert.NotNull(items);

            Assert.All(items!, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetAllProducts_FilterByCategory_ReturnsOnlyThatCategory()
        {
            const int categoryId = 1;

            var res = await _client.GetAsync($"/api/products?categoryId={categoryId}");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
                return;

            var items = await res.Content.ReadFromJsonAsync<List<ProductResponse>>();
            Assert.NotNull(items);

            Assert.All(items!, p => Assert.Equal(categoryId, p.CategoryId));
        }

        [Fact]
        public async Task GetAllProducts_SearchTerm_ReturnsMatchingResults()
        {
            const string term = "laptop"; // adjust to your seed

            var res = await _client.GetAsync($"/api/products?searchTerm={Uri.EscapeDataString(term)}");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
                return;

            var items = await res.Content.ReadFromJsonAsync<List<ProductResponse>>();
            Assert.NotNull(items);

            Assert.All(items!, p =>
                Assert.True(
                    p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)));
        }

        [Fact]
        public async Task GetAllProducts_SortByNameAscending_IsSorted()
        {
            var res = await _client.GetAsync("/api/products?sortBy=Name&sortDescending=false&page=1&pageSize=50");
            var body = await res.Content.ReadAsStringAsync();

            Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Status: {(int)res.StatusCode}\n{body}");

            if (res.StatusCode == HttpStatusCode.NoContent)
                return;

            var items = (await res.Content.ReadFromJsonAsync<List<ProductResponse>>())!;
            Assert.True(items.Count > 1);

            var sorted = items.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Select(x => x.Id).ToList();
            var actual = items.Select(x => x.Id).ToList();

            Assert.Equal(sorted, actual);
        }

        [Fact]
        public async Task GetAllProducts_InvalidPage_Returns400ValidationProblem()
        {
            var res = await _client.GetAsync("/api/products?page=0&pageSize=10");
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

            // Optional: assert it's a validation problem payload
            Assert.Contains("errors", body, StringComparison.OrdinalIgnoreCase);
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
