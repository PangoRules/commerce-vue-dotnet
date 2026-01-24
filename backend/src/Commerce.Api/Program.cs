using Commerce.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiControllers()
    .AddApiCors()
    .AddApiDocs()
    .AddPersistence(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();

app
    .UseApiDocs()
    .UseApiHttps()
    .UseApiCors()
    .MapApiControllers();

app.Run();
