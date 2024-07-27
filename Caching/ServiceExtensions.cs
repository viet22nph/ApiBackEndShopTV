using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Models.Settings;

namespace Caching
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
        {
            #region Configure
            services.Configure<RedisSettings>(config.GetSection("RedisSettings"));
            #endregion

            #region Services
            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<RedisSettings>>().Value);
            services.AddScoped<IRedisConnectionWrapper, RedisConnectionWrapper>();
            services.AddScoped<ICacheManager, RedisCacheManager>();
            #endregion

            return services;
        }
    }
}
