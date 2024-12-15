namespace FoxBlog.Application.PostEntity;

public interface IPostContext
{
    public Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<Post?> GetAsync(PostKey key, CancellationToken cancellationToken = default);
}
