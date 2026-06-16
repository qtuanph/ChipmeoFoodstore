using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ChipmeoApis.Usecase.Utils;

public static class DistributedCacheExtensions
{
    public static async Task<T> GetOrSetAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var cached = await cache.GetAsync(key, cancellationToken);
        if (cached != null)
        {
            var deserialized = JsonSerializer.Deserialize<T>(cached);
            if (deserialized != null) return deserialized;
        }

        var result = await factory();
        var serialized = JsonSerializer.SerializeToUtf8Bytes(result);
        await cache.SetAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        }, cancellationToken);

        return result;
    }
}
