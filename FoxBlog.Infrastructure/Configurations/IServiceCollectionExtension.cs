using FoxBlog.Application.CategoryEntity;
using FoxBlog.Application.PostEntity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoxBlog.Infrastructure.Configurations;

public static partial class IServiceCollectionExtension
{
    public static IServiceCollection AddContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<ContentOptions>(configuration);
        services.AddScoped<IPostContext, PostContext>();
        services.AddScoped<ICategoryContext, CategoryContext>();
        services.AddScoped<IPostHeaderRepository, PostHeaderRepository>();
        services.AddScoped<IPostContentReader, PostContentReader>();

        return services;
    }
}
