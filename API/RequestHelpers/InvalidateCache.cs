using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers;

[AttributeUsage(AttributeTargets.Method)]
public class InvalidateCache(string pattern) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // after the action is executed (eg, once new product is added => invalidate cache)
        var resultContext = await next();

        if (resultContext.Exception == null || resultContext.ExceptionHandled)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            await cacheService.RemoveCacheByPattern(pattern);
        }
    }
}
