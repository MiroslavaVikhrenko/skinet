namespace Core.Interfaces;

public interface IResponseCacheService
{
    // the key we use to retrieve cached thing from Redis
    Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
    Task<string?> GetCachedResponseAsync(string cacheKey);
    Task RemoveCacheByPattern(string pattern); // remove specific items from cache based on a pattern
}
