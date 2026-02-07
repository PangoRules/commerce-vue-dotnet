using System.Text.Json.Serialization;

namespace Commerce.Shared.Enums;


public enum DbResultOption
{
    Success,
    NotFound,
    AlreadyExists,
    Invalid,
    Conflict,
    Error
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImageAssetType
{
    Product,
    Category
}
