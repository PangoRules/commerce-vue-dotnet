using Amazon.Runtime;
using Amazon.S3;
using Commerce.Api.Configurations;
using Commerce.Api.Storage;
using Microsoft.Extensions.Options;


namespace Commerce.Api.Extensions;

public static class StorageExtensions
{
  public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration config)
  {
    services.Configure<StorageOptions>(config.GetSection("Storage"));

    services.AddSingleton<IAmazonS3>(sp =>
    {
      var opt = sp.GetRequiredService<IOptions<StorageOptions>>().Value;

      var s3Config = new AmazonS3Config
      {
        ServiceURL = opt.Endpoint,
        ForcePathStyle = true,
        UseHttp = !opt.UseSsl
      };

      var creds = new BasicAWSCredentials(opt.AccessKey, opt.SecretKey);
      return new AmazonS3Client(creds, s3Config);
    });

    services.AddScoped<IObjectStorageService, MinioS3StorageService>();


    return services;
  }
}
