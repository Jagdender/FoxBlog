using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoxBlog.Application.CategoryEntity;

[JsonConverter(typeof(CategoryKeyConverter))]
public readonly record struct CategoryKey(int Value)
{
    public override readonly string ToString() => Value.ToString();

    public static implicit operator string(CategoryKey post) => post.Value.ToString();
}

internal sealed class CategoryKeyConverter : JsonConverter<CategoryKey>
{
    public override CategoryKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return new CategoryKey(reader.GetInt32());
    }

    public override void Write(
        Utf8JsonWriter writer,
        CategoryKey value,
        JsonSerializerOptions options
    )
    {
        writer.WriteNumberValue(value.Value);
    }
}
