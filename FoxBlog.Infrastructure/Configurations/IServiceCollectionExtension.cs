using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoxBlog.Infrastructure.Configurations
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddContext(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<ContentContext>(configuration);
            return services;
        }
    }
}
