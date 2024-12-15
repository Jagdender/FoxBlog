using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoxBlog.Application.PostEntity;

[JsonConverter(typeof(PostKeyConverter))]
public readonly record struct PostKey(int Value)
{
    public override readonly string ToString() => Value.ToString();

    public static implicit operator string(PostKey post) => post.Value.ToString();
}

internal sealed class PostKeyConverter : JsonConverter<PostKey>
{
    public override PostKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return new PostKey(reader.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, PostKey value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
