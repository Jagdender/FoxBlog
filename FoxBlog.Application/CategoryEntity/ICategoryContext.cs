namespace FoxBlog.Application.CategoryEntity;

public interface ICategoryContext
{
    public Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<Category?> GetAsync(CategoryKey key, CancellationToken cancellationToken = default);
}
