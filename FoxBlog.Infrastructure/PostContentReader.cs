using FoxBlog.Application.PostEntity;
using Microsoft.Extensions.Options;

namespace FoxBlog.Infrastructure;

internal sealed class PostContentReader(IOptionsSnapshot<ContentOptions> options)
    : IPostContentReader
{
    private readonly ContentOptions options = options.Value;

    public async Task<string?> ReadAsync(
        PostKey post,
        CancellationToken cancellationToken = default
    )
    {
        string? filename = options.GetPostMarkdownFilePath(post);

        if (filename is null || !File.Exists(filename))
            return null;

        return await File.ReadAllTextAsync(filename, cancellationToken);
    }
}
