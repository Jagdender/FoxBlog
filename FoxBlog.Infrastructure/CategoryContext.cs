using System.Text.Json;
using FoxBlog.Application.CategoryEntity;
using Microsoft.Extensions.Options;

namespace FoxBlog.Infrastructure;

internal sealed class CategoryContext(IOptionsSnapshot<ContentOptions> options) : ICategoryContext
{
    private readonly ContentOptions options = options.Value;

    public async Task<IEnumerable<Category>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (options.RecordsFilePath is null)
            return [];

        byte[] bytes = await File.ReadAllBytesAsync(options.RecordsFilePath, cancellationToken);
        Utf8JsonReader reader = new(bytes);

        var categories =
            JsonElement
                .ParseValue(ref reader)
                .GetProperty("Categories")
                .Deserialize<IEnumerable<Category>>() ?? [];

        return categories.Select(SetKey);
    }

    public async Task<Category?> GetAsync(
        CategoryKey key,
        CancellationToken cancellationToken = default
    )
    {
        var categories = await GetAllAsync(cancellationToken);

        return categories.ElementAtOrDefault(key.Value);
    }

    private static Category SetKey(Category category, int index)
    {
        typeof(Category)
            .GetProperty(nameof(Category.Key))!
            .SetValue(category, new CategoryKey(index));

        return category;
    }
}
