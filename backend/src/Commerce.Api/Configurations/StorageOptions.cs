namespace Commerce.Api.Configurations;

public class StorageOptions
{
  public string Provider { get; init; } = "Minio";
  public string Endpoint { get; init; } = "";        // http://minio:9000 (inside docker)
  public string AccessKey { get; init; } = "";
  public string SecretKey { get; init; } = "";
  public string Bucket { get; init; } = "";
  public string Region { get; init; } = "us-east-1";
  public bool UseSsl { get; init; } = false;

  // IMPORTANT: what the browser uses (e.g. http://localhost:9000)
  public string PublicBaseUrl { get; init; } = "";
}
