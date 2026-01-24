# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Put NuGet packages somewhere predictable
ENV NUGET_PACKAGES=/root/.nuget/packages

# copy sln + csproj files first
COPY Commerce.sln ./
COPY src/Commerce.Api/*.csproj src/Commerce.Api/
COPY src/Commerce.Repositories/*.csproj src/Commerce.Repositories/
COPY src/Commerce.Services/*.csproj src/Commerce.Services/
COPY src/Commerce.Shared/*.csproj src/Commerce.Shared/
COPY tests/Commerce.UnitTests/*.csproj tests/Commerce.UnitTests/

# Restore (cache NuGet packages)
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore src/Commerce.Api/Commerce.Api.csproj

# copy everything else
COPY . ./

# Publish (let it restore if needed)
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/Commerce.Api/Commerce.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Commerce.Api.dll"]
