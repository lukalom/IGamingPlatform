using System.Text.Json;
using System.Text.Json.Serialization;

namespace IGamingPlatform.Shared.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize<T>(this T value) => JsonSerializer.Serialize(value, Options);

    public static T? Deserialize<T>(this string? value) => string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value, Options);
}
