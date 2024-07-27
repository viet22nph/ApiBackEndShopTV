using Caching;
using StackExchange.Redis;

namespace WebApi.Middlewares
{
    public class VisitTrackingMiddleware
    {
        
            private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        public VisitTrackingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
            {
                _next = next;
            _serviceProvider = serviceProvider;
            }

            public async Task InvokeAsync(HttpContext context)
            {
            using (var scope = _serviceProvider.CreateScope())
            {
                var redis = scope.ServiceProvider.GetRequiredService<ICacheManager>();

                await redis.IncrementVisitCountAsync();

                // Theo dõi khách truy cập duy nhất bằng địa chỉ IP
                var ip = context.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(ip))
                {
                    await redis.IncrementUniqueVisitorCountAsync(ip);
                }
            }

            await _next(context);
            }
        
    }
}
