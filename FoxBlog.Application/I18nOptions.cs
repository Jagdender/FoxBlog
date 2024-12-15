using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoxBlog.Application;

[JsonConverter(typeof(I18nOptionsConverter))]
public readonly record struct I18nStringOptions(string Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }
}

internal sealed class I18nOptionsConverter : JsonConverter<I18nStringOptions>
{
    public override I18nStringOptions Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new I18nStringOptions(reader.GetString()!);
        }
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var block = JsonElement.ParseValue(ref reader);
            string language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (block.TryGetProperty(language, out var value))
                return new(value.GetString()!);

            value = block.GetProperty("default");
            return new(value.GetString()!);
        }

        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        I18nStringOptions value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Value);
    }
}
