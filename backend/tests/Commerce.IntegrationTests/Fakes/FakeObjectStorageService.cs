using Commerce.Api.Storage;

namespace Commerce.IntegrationTests.Fakes;

/// <summary>
/// In-memory fake storage service for integration tests.
/// Stores files in a dictionary instead of MinIO.
/// </summary>
public sealed class FakeObjectStorageService : IObjectStorageService
{
    private readonly Dictionary<string, StoredObject> _objects = new();

    public Task UploadAsync(string objectKey, Stream content, string contentType, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        content.CopyTo(ms);

        _objects[objectKey] = new StoredObject(ms.ToArray(), contentType);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string objectKey, CancellationToken ct = default)
    {
        _objects.Remove(objectKey);
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string objectKey)
    {
        return $"http://fake-storage/{objectKey}";
    }

    public Task<StorageObjectResult?> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        if (!_objects.TryGetValue(objectKey, out var obj))
            return Task.FromResult<StorageObjectResult?>(null);

        var stream = new MemoryStream(obj.Data);
        return Task.FromResult<StorageObjectResult?>(
            new StorageObjectResult(stream, obj.ContentType, obj.Data.Length));
    }

    public Task<bool> ExistsAsync(string objectKey, CancellationToken ct = default)
    {
        return Task.FromResult(_objects.ContainsKey(objectKey));
    }

    // Helper to check test assertions
    public bool HasObject(string objectKey) => _objects.ContainsKey(objectKey);
    public int ObjectCount => _objects.Count;

    private sealed record StoredObject(byte[] Data, string ContentType);
}
