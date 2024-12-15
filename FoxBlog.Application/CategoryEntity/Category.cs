namespace FoxBlog.Application.CategoryEntity;

public sealed class Category
{
    public CategoryKey Key { get; init; }
    public required I18nStringOptions Name { get; init; }
}
