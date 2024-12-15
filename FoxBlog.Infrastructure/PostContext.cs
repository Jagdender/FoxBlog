using System.Text.Json;
using FoxBlog.Application.CategoryEntity;
using FoxBlog.Application.PostEntity;
using Microsoft.Extensions.Options;

namespace FoxBlog.Infrastructure;

internal sealed class PostContext(
    IOptionsSnapshot<ContentOptions> options,
    ICategoryContext categoryContext
) : IPostContext
{
    private readonly ContentOptions options = options.Value;

    public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(options.RecordsFilePath))
            return [];

        byte[] bytes = await File.ReadAllBytesAsync(options.RecordsFilePath, cancellationToken);
        Utf8JsonReader reader = new(bytes);

        var posts =
            JsonElement.ParseValue(ref reader).GetProperty("Posts").Deserialize<IEnumerable<Post>>()
            ?? [];

        var categories = await categoryContext.GetAllAsync(cancellationToken);

        return posts.Select(SetKey).Select(post => SetCategory(post, categories));
    }

    public async Task<Post?> GetAsync(PostKey key, CancellationToken cancellationToken)
    {
        var posts = await GetAllAsync(cancellationToken);

        return posts.FirstOrDefault(p => p.Key == key);
    }

    private static Post SetKey(Post post, int index)
    {
        typeof(Post).GetProperty(nameof(Post.Key))?.SetValue(post, new PostKey(index));

        return post;
    }

    private static Post SetCategory(Post post, IEnumerable<Category> categories)
    {
        typeof(Post)
            .GetProperty(nameof(Post.Category))
            ?.SetValue(post, categories.First(c => c.Key == post.CategoryKey));

        return post;
    }
}
