using FoxBlog.Application.PostEntity;
using Microsoft.Extensions.Options;

namespace FoxBlog.Infrastructure;

internal sealed class PostHeaderRepository(IOptionsSnapshot<ContentOptions> options)
    : IPostHeaderRepository
{
    private readonly ContentOptions options = options.Value;

    public Stream? GetHeaderImgFile(PostKey post)
    {
        string? header = options.GetPostHeaderFilePath(post);

        return header is null ? null : File.OpenRead(header);
    }

    public string? GetHeaderImgUrl(PostKey post)
    {
        string? header = options.GetPostHeaderFilePath(post);

        return header is null ? null : $"/header/{post}";
    }
}
