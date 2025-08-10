using System.Text.Json;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class ResponseCacheService(IConnectionMultiplexer redis) : IResponseCacheService
{
    // for caching use different db for this inside redis (db 1), the one that cart is using is default one 0
    private readonly IDatabase _database = redis.GetDatabase(1);
    // to store API response (eg. paginated list of products) - object
    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        // strore in string 
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        // serialize c# object into a string
        var serializedResponse = JsonSerializer.Serialize(response, options);
        await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);
        if (cachedResponse.IsNullOrEmpty) return null;
        return cachedResponse;
    }

    // pattern: api/products|
    public async Task RemoveCacheByPattern(string pattern)
    {
        // redis
        var server = redis.GetServer(redis.GetEndPoints().First());
        // array of keys that match the pattern that we're passing as parameter (* for wildcard)
        var keys = server.Keys(database: 1, pattern: $"*{pattern}*").ToArray();

        if (keys.Length != 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
