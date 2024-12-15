using System.Text.Json;
using System.Text.Json.Serialization;
using FoxBlog.Application.CategoryEntity;

namespace FoxBlog.Application.PostEntity;

public sealed class Post
{
    public PostKey Key { get; init; }

    [JsonPropertyName("Category")]
    public required CategoryKey CategoryKey { get; init; }
    public required I18nStringOptions Title { get; init; }
    public string[] Tags { get; init; } = [];
    public string? Password { get; init; } = null;

    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime DateTime { get; init; }

    [JsonIgnore]
    public Category Category { get; init; } = null!;
    public bool IsPrivate => Password is not null;
}

file sealed class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return DateTime.Parse(reader.GetString()!);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}
