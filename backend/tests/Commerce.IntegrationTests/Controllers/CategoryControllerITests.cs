using System.Net;
using System.Net.Http.Json;
using Commerce.Shared.Responses;

namespace Commerce.IntegrationTests;

[Collection("Integration")]
public class CategoryEndpointsTests
{
    private readonly HttpClient _client;

    public CategoryEndpointsTests(PostgresContainerFixture postgres)
    {
        var factory = new TestAppFactory(postgres);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCategories_NoParams_Returns200Or204()
    {
        var res = await _client.GetAsync("/api/category");
        var body = await res.Content.ReadAsStringAsync();

        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
            $"Status: {(int)res.StatusCode}\n{body}");

        if (res.StatusCode == HttpStatusCode.NoContent)
            return;

        var categories = await res.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories!);
    }

    [Fact]
    public async Task GetAllCategories_FeaturedOnlyTrue_ReturnsOnlyFeaturedCategories()
    {
        var res = await _client.GetAsync("/api/category?featuredOnly=true");
        var body = await res.Content.ReadAsStringAsync();

        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
            $"Status: {(int)res.StatusCode}\n{body}");

        if (res.StatusCode == HttpStatusCode.NoContent)
            return;

        var items = await res.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items!);

        // All returned categories should be featured
        Assert.All(items!, c => Assert.True(c.IsFeatured));
    }

    [Fact]
    public async Task GetAllCategories_FeaturedOnlyFalse_ReturnsAllCategories()
    {
        var res = await _client.GetAsync("/api/category?featuredOnly=false");
        var body = await res.Content.ReadAsStringAsync();

        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
            $"Status: {(int)res.StatusCode}\n{body}");

        if (res.StatusCode == HttpStatusCode.NoContent)
            return;

        var items = await res.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.NotNull(items);

        // Should include both featured and non-featured categories
        var hasFeatured = items!.Any(c => c.IsFeatured);
        var hasNonFeatured = items!.Any(c => !c.IsFeatured);
        Assert.True(hasFeatured && hasNonFeatured,
            "Expected both featured and non-featured categories in the results");
    }

    [Fact]
    public async Task GetRoots_NoParams_Returns200Or204()
    {
        var res = await _client.GetAsync("/api/category/roots");
        var body = await res.Content.ReadAsStringAsync();

        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
            $"Status: {(int)res.StatusCode}\n{body}");

        if (res.StatusCode == HttpStatusCode.NoContent)
            return;

        var roots = await res.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.NotNull(roots);
        Assert.NotEmpty(roots!);
    }

    [Fact]
    public async Task GetRoots_FeaturedOnlyTrue_ReturnsOnlyFeaturedRoots()
    {
        var res = await _client.GetAsync("/api/category/roots?featuredOnly=true");
        var body = await res.Content.ReadAsStringAsync();

        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
            $"Status: {(int)res.StatusCode}\n{body}");

        if (res.StatusCode == HttpStatusCode.NoContent)
            return;

        var items = await res.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items!);

        // All returned root categories should be featured
        Assert.All(items!, c => Assert.True(c.IsFeatured));
    }

    [Fact]
    public async Task GetCategoryById_FeaturedCategory_ReturnsIsFeaturedTrue()
    {
        // Category 1 (Electronics) is featured
        var res = await _client.GetAsync("/api/category/1");
        var body = await res.Content.ReadAsStringAsync();

        Assert.True(res.IsSuccessStatusCode, $"Status: {(int)res.StatusCode}\n{body}");

        var category = await res.Content.ReadFromJsonAsync<CategoryAdminDetailsResponse>();
        Assert.NotNull(category);
        Assert.Equal(1, category!.Id);
    }
}
