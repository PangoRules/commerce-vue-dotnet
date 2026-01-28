using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Commerce.Api.Configurations;

namespace Commerce.Api.Storage;

/// <summary>
/// Abstraction for object storage operations (S3-compatible).
/// Provides methods for uploading, deleting, and retrieving public URLs for stored objects.
/// </summary>
/// <remarks>
/// This interface is designed to work with any S3-compatible storage provider,
/// including AWS S3, MinIO, DigitalOcean Spaces, and others.
/// </remarks>
public interface IObjectStorageService
{
  /// <summary>
  /// Uploads an object to the storage bucket.
  /// </summary>
  /// <param name="objectKey">
  /// The unique key (path) for the object in the bucket.
  /// Example: "products/123/image.webp" or "avatars/user-456.png".
  /// </param>
  /// <param name="content">
  /// The readable stream containing the object data to upload.
  /// The stream position should be at the beginning.
  /// </param>
  /// <param name="contentType">
  /// The MIME type of the content (e.g., "image/webp", "application/pdf").
  /// Defaults to "application/octet-stream" if null or empty.
  /// </param>
  /// <param name="ct">Optional cancellation token to cancel the operation.</param>
  /// <returns>A task that completes when the upload is finished.</returns>
  /// <exception cref="ArgumentException">
  /// Thrown when <paramref name="objectKey"/> is null or whitespace,
  /// or when <paramref name="content"/> is null or not readable.
  /// </exception>
  Task UploadAsync(
      string objectKey,
      Stream content,
      string contentType,
      CancellationToken ct = default);

  /// <summary>
  /// Deletes an object from the storage bucket.
  /// </summary>
  /// <param name="objectKey">
  /// The unique key (path) of the object to delete.
  /// Example: "products/123/image.webp".
  /// </param>
  /// <param name="ct">Optional cancellation token to cancel the operation.</param>
  /// <returns>A task that completes when the deletion is finished.</returns>
  /// <exception cref="ArgumentException">
  /// Thrown when <paramref name="objectKey"/> is null or whitespace.
  /// </exception>
  /// <remarks>
  /// This operation is idempotent - deleting a non-existent object does not throw an error.
  /// </remarks>
  Task DeleteAsync(
      string objectKey,
      CancellationToken ct = default);

  /// <summary>
  /// Generates the public URL for accessing a stored object.
  /// </summary>
  /// <param name="objectKey">
  /// The unique key (path) of the object.
  /// Example: "products/123/image.webp".
  /// </param>
  /// <returns>
  /// The fully-qualified public URL to access the object.
  /// Example: "http://localhost:9000/commerce-assets/products/123/image.webp".
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Thrown when <paramref name="objectKey"/> is null or whitespace.
  /// </exception>
  /// <remarks>
  /// The returned URL assumes the bucket has public read access configured.
  /// For private buckets, use pre-signed URLs instead.
  /// </remarks>
  string GetPublicUrl(string objectKey);

  /// <summary>
  /// Retrieves the object content as a stream for proxying to clients.
  /// </summary>
  /// <param name="objectKey">
  /// The unique key (path) of the object.
  /// Example: "products/123/image.webp".
  /// </param>
  /// <param name="ct">Optional cancellation token to cancel the operation.</param>
  /// <returns>
  /// A result containing the response stream and content type if successful,
  /// or null if the object does not exist.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Thrown when <paramref name="objectKey"/> is null or whitespace.
  /// </exception>
  /// <remarks>
  /// The caller is responsible for disposing the returned stream.
  /// This method is designed for proxy endpoints that serve private bucket content.
  /// </remarks>
  Task<StorageObjectResult?> GetObjectAsync(string objectKey, CancellationToken ct = default);

  /// <summary>
  /// Checks if an object exists in the storage bucket.
  /// </summary>
  /// <param name="objectKey">The unique key (path) of the object.</param>
  /// <param name="ct">Optional cancellation token.</param>
  /// <returns>True if the object exists, false otherwise.</returns>
  Task<bool> ExistsAsync(string objectKey, CancellationToken ct = default);
}

/// <summary>
/// Result of retrieving an object from storage.
/// </summary>
/// <param name="Stream">The object content stream. Caller must dispose.</param>
/// <param name="ContentType">The MIME type of the object.</param>
/// <param name="ContentLength">The size of the object in bytes.</param>
public sealed record StorageObjectResult(
    Stream Stream,
    string ContentType,
    long ContentLength) : IDisposable
{
  public void Dispose() => Stream.Dispose();
}

/// <summary>
/// MinIO-backed implementation of <see cref="IObjectStorageService"/> using the AWS S3 SDK.
/// </summary>
/// <remarks>
/// <para>
/// This service uses the AWS SDK for .NET to communicate with MinIO's S3-compatible API.
/// MinIO implements the S3 protocol, allowing standard S3 clients to work seamlessly.
/// </para>
/// <para>
/// Configuration is provided via <see cref="StorageOptions"/>:
/// <list type="bullet">
///   <item><description><c>Endpoint</c>: MinIO server URL (e.g., http://minio:9000)</description></item>
///   <item><description><c>Bucket</c>: Target bucket name for all operations</description></item>
///   <item><description><c>PublicBaseUrl</c>: Browser-accessible URL for public object links</description></item>
/// </list>
/// </para>
/// </remarks>
/// <param name="s3">The AWS S3 client configured for MinIO.</param>
/// <param name="options">Storage configuration options.</param>
public sealed class MinioS3StorageService(
    IAmazonS3 s3,
    IOptions<StorageOptions> options) : IObjectStorageService
{
  private readonly IAmazonS3 _s3 = s3;
  private readonly StorageOptions _options = options.Value;

  /// <inheritdoc/>
  public async Task UploadAsync(
      string objectKey,
      Stream content,
      string contentType,
      CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(objectKey))
      throw new ArgumentException("Object key is required.", nameof(objectKey));

    if (content is null || !content.CanRead)
      throw new ArgumentException("Content stream must be readable.", nameof(content));

    var request = new PutObjectRequest
    {
      BucketName = _options.Bucket,
      Key = objectKey,
      InputStream = content,
      ContentType = string.IsNullOrWhiteSpace(contentType)
            ? "application/octet-stream"
            : contentType
    };

    await _s3.PutObjectAsync(request, ct);
  }

  /// <inheritdoc/>
  public Task DeleteAsync(
      string objectKey,
      CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(objectKey))
      throw new ArgumentException("Object key is required.", nameof(objectKey));

    return _s3.DeleteObjectAsync(_options.Bucket, objectKey, ct);
  }

  /// <inheritdoc/>
  public string GetPublicUrl(string objectKey)
  {
    if (string.IsNullOrWhiteSpace(objectKey))
      throw new ArgumentException("Object key is required.", nameof(objectKey));

    // Example:
    // http://localhost:9000/commerce-assets/products/123/image.webp
    var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
    return $"{baseUrl}/{_options.Bucket}/{objectKey}";
  }

  /// <inheritdoc/>
  public async Task<StorageObjectResult?> GetObjectAsync(string objectKey, CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(objectKey))
      throw new ArgumentException("Object key is required.", nameof(objectKey));

    try
    {
      var request = new GetObjectRequest
      {
        BucketName = _options.Bucket,
        Key = objectKey
      };

      var response = await _s3.GetObjectAsync(request, ct);

      return new StorageObjectResult(
          response.ResponseStream,
          response.Headers.ContentType,
          response.Headers.ContentLength);
    }
    catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      return null;
    }
  }

  /// <inheritdoc/>
  public async Task<bool> ExistsAsync(string objectKey, CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(objectKey))
      throw new ArgumentException("Object key is required.", nameof(objectKey));

    try
    {
      var request = new GetObjectMetadataRequest
      {
        BucketName = _options.Bucket,
        Key = objectKey
      };

      await _s3.GetObjectMetadataAsync(request, ct);
      return true;
    }
    catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      return false;
    }
  }
}
