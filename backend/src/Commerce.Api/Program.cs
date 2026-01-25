using Commerce.Api.Extensions;
using Commerce.Repositories.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiControllers()
    .AddApiCors()
    .AddApiDocs()
    .AddPersistence(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CommerceDbContext>();

    app.Logger.LogInformation("Applying EF Core migrations (Development)...");
    await dbContext.Database.MigrateAsync();
    app.Logger.LogInformation("EF Core migrations applied.");
}

app
    .UseApiDocs()
    .UseApiHttps()
    .UseApiCors()
    .MapApiControllers();

app.Run();

public partial class Program { }
