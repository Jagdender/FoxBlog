namespace FoxBlog.Application.PostEntity;

public interface IPostContentReader
{
    public Task<string?> ReadAsync(PostKey post, CancellationToken cancellationToken = default);
}
