using Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Settings;
using System.Text;

namespace WebApi.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;    

        public CacheAttribute(int time = 1800)
        {
            _timeToLiveSeconds = time;

        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Sử lý cache
            var cacheConfig = context.HttpContext.RequestServices.GetRequiredService<RedisSettings>();
            if(!cacheConfig.RedisEnable)
            {
                await next();
                return;
            }
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheManager>();
            var cacheKey = GenerateCacheKey(context.HttpContext.Request);


            var cacheRes = await cacheService.GetAsync(cacheKey);
            if(!string.IsNullOrWhiteSpace(cacheRes))
            {
                var contentResult = new ContentResult
                {
                    Content = cacheRes,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }    

            var excutedContext = await next();
            if(excutedContext.Result is OkObjectResult objectResult) {
                await cacheService.SetAsync(cacheKey, objectResult.Value, _timeToLiveSeconds);
            }
            
        }

        private static string GenerateCacheKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach( var (key, value) in request.Query.OrderBy(x=>x.Key) )
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            keyBuilder.Remove(0, 1);
            return keyBuilder.ToString();
        }
    }
}
