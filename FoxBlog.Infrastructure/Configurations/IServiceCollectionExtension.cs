using FoxBlog.Application.Contexts;
using FoxBlog.Infrastructure.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoxBlog.Infrastructure.Configurations
{
    public static partial class IServiceCollectionExtension
    {
        public static IServiceCollection AddContext(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<ContentOptions>(configuration);
            services.AddScoped<IPostContext, PostContext>();

            return services;
        }
    }
}
