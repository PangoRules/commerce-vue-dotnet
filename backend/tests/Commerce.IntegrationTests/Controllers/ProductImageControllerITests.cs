using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.IntegrationTests.Controllers;

[Collection("Integration")]
public class ProductImageControllerITests
{
    private readonly HttpClient _client;
    private readonly TestAppFactory _factory;

    public ProductImageControllerITests(PostgresContainerFixture postgres)
    {
        _factory = new TestAppFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProductImages_ReturnsNoContent_WhenNoImages()
    {
        // Product 1001 exists from seed data but has no images
        var res = await _client.GetAsync("/api/product/1001/images");

        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task UploadImage_Returns201_AndCreatesImageMetadata()
    {
        // Arrange - create fake image content
        var imageContent = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG magic bytes
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(imageContent);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "test-image.png");

        // Act
        var res = await _client.PostAsync("/api/product/1001/images", content);
        var body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.True(res.StatusCode == HttpStatusCode.Created,
            $"Expected 201, got {(int)res.StatusCode}: {body}");

        var image = await res.Content.ReadFromJsonAsync<ProductImageResponse>();
        Assert.NotNull(image);
        Assert.Equal(1001, image!.ProductId);
        Assert.Equal("test-image.png", image.FileName);
        Assert.Equal("image/png", image.ContentType);
        Assert.True(image.IsPrimary); // First image should be primary
        Assert.Equal(0, image.DisplayOrder);
        Assert.StartsWith("/api/productimage/", image.Url);

        // Verify storage was called
        Assert.True(_factory.FakeStorage.ObjectCount > 0);
    }

    [Fact]
    public async Task UploadImage_Returns400_WhenNoFile()
    {
        using var content = new MultipartFormDataContent();

        var res = await _client.PostAsync("/api/product/1001/images", content);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task UploadImage_Returns400_WhenInvalidContentType()
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([0x00, 0x01, 0x02]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "file", "document.pdf");

        var res = await _client.PostAsync("/api/product/1001/images", content);
        var body = await res.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Contains("Invalid file type", body);
    }

    [Fact]
    public async Task UploadImage_Returns404_WhenProductDoesNotExist()
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "test.png");

        var res = await _client.PostAsync("/api/product/999999/images", content);

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task GetImage_Returns404_WhenImageDoesNotExist()
    {
        var fakeId = Guid.NewGuid();

        var res = await _client.GetAsync($"/api/productimage/{fakeId}");

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task UploadAndGetImage_ReturnsImageStream()
    {
        // Upload
        var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG magic
        using var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(imageBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        uploadContent.Add(fileContent, "file", "photo.jpg");

        var uploadRes = await _client.PostAsync("/api/product/1002/images", uploadContent);
        Assert.Equal(HttpStatusCode.Created, uploadRes.StatusCode);

        var uploaded = await uploadRes.Content.ReadFromJsonAsync<ProductImageResponse>();
        Assert.NotNull(uploaded);

        // Get via proxy
        var getRes = await _client.GetAsync($"/api/productimage/{uploaded!.Id}");

        Assert.Equal(HttpStatusCode.OK, getRes.StatusCode);
        Assert.Equal("image/jpeg", getRes.Content.Headers.ContentType?.MediaType);

        var downloadedBytes = await getRes.Content.ReadAsByteArrayAsync();
        Assert.Equal(imageBytes, downloadedBytes);
    }

    [Fact]
    public async Task GetImageMetadata_Returns200_WithImageDetails()
    {
        // Upload first
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "metadata-test.png");

        var uploadRes = await _client.PostAsync("/api/product/1003/images", content);
        var uploaded = await uploadRes.Content.ReadFromJsonAsync<ProductImageResponse>();

        // Get metadata
        var res = await _client.GetAsync($"/api/productimage/{uploaded!.Id}/metadata");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var metadata = await res.Content.ReadFromJsonAsync<ProductImageResponse>();
        Assert.NotNull(metadata);
        Assert.Equal(uploaded.Id, metadata!.Id);
        Assert.Equal("metadata-test.png", metadata.FileName);
    }

    [Fact]
    public async Task DeleteImage_Returns204_AndRemovesFromStorage()
    {
        // Upload
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "to-delete.png");

        var uploadRes = await _client.PostAsync("/api/product/2001/images", content);
        var uploaded = await uploadRes.Content.ReadFromJsonAsync<ProductImageResponse>();

        var countBefore = _factory.FakeStorage.ObjectCount;

        // Delete
        var deleteRes = await _client.DeleteAsync($"/api/productimage/{uploaded!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);

        // Verify storage object was removed
        Assert.Equal(countBefore - 1, _factory.FakeStorage.ObjectCount);

        // Verify metadata is gone
        var getRes = await _client.GetAsync($"/api/productimage/{uploaded.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task DeleteImage_Returns404_WhenNotFound()
    {
        var res = await _client.DeleteAsync($"/api/productimage/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task SetPrimary_Returns204_AndUpdatesPrimaryFlag()
    {
        // Upload two images
        for (int i = 0; i < 2; i++)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            content.Add(fileContent, "file", $"primary-test-{i}.png");

            await _client.PostAsync("/api/product/2002/images", content);
        }

        // Get images
        var listRes = await _client.GetAsync("/api/product/2002/images");
        var images = await listRes.Content.ReadFromJsonAsync<List<ProductImageResponse>>();
        Assert.NotNull(images);
        Assert.True(images!.Count >= 2);

        var secondImage = images.First(i => !i.IsPrimary);

        // Set second as primary
        var res = await _client.PutAsync($"/api/productimage/{secondImage.Id}/primary", null);

        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);

        // Verify
        var metadataRes = await _client.GetAsync($"/api/productimage/{secondImage.Id}/metadata");
        var updated = await metadataRes.Content.ReadFromJsonAsync<ProductImageResponse>();
        Assert.True(updated!.IsPrimary);
    }

    [Fact]
    public async Task ReorderImages_Returns204_AndUpdatesDisplayOrder()
    {
        // Upload three images to product 2003
        var imageIds = new List<Guid>();
        for (int i = 0; i < 3; i++)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            content.Add(fileContent, "file", $"reorder-{i}.png");

            var uploadRes = await _client.PostAsync("/api/product/2003/images", content);
            var uploaded = await uploadRes.Content.ReadFromJsonAsync<ProductImageResponse>();
            imageIds.Add(uploaded!.Id);
        }

        // Reverse the order
        var reversedIds = imageIds.AsEnumerable().Reverse().ToList();
        var reorderRequest = new ReorderImagesRequest { ImageIds = reversedIds };

        var res = await _client.PutAsJsonAsync("/api/product/2003/images/reorder", reorderRequest);

        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);

        // Verify order changed
        var listRes = await _client.GetAsync("/api/product/2003/images");
        var images = await listRes.Content.ReadFromJsonAsync<List<ProductImageResponse>>();

        Assert.Equal(reversedIds[0], images![0].Id);
        Assert.Equal(0, images[0].DisplayOrder);
        Assert.Equal(reversedIds[1], images[1].Id);
        Assert.Equal(1, images[1].DisplayOrder);
    }

    [Fact]
    public async Task ReorderImages_Returns400_WhenNoImageIds()
    {
        var request = new ReorderImagesRequest { ImageIds = [] };

        var res = await _client.PutAsJsonAsync("/api/product/1001/images/reorder", request);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task MultipleUploads_SecondImageIsNotPrimary()
    {
        // First upload
        using var content1 = new MultipartFormDataContent();
        var fileContent1 = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
        fileContent1.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content1.Add(fileContent1, "file", "first.png");

        var res1 = await _client.PostAsync("/api/product/3001/images", content1);
        var first = await res1.Content.ReadFromJsonAsync<ProductImageResponse>();
        Assert.True(first!.IsPrimary);

        // Second upload
        using var content2 = new MultipartFormDataContent();
        var fileContent2 = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
        fileContent2.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content2.Add(fileContent2, "file", "second.png");

        var res2 = await _client.PostAsync("/api/product/3001/images", content2);
        var second = await res2.Content.ReadFromJsonAsync<ProductImageResponse>();

        Assert.False(second!.IsPrimary);
        Assert.Equal(1, second.DisplayOrder);
    }
}
