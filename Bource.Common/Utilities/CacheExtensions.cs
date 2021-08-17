using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Common.Utilities
{
    public static class CacheExtensions
    {
        public static async Task<T> GetValueAsync<T>(this IDistributedCache cache, string name, CancellationToken cancellationToken = default)
        {
            var objectCache = await cache.GetStringAsync(name, cancellationToken);
            if (objectCache is null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(objectCache);
        }

        public static async Task SetValueAsync(this IDistributedCache cache, string name, object value, double minute, CancellationToken cancellationToken = default)
        {
            var objectCache = await cache.GetStringAsync(name, cancellationToken);
            if (!(objectCache is null))
                await cache.RemoveAsync(name);

            var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(minute));

            await cache.SetStringAsync(name, JsonConvert.SerializeObject(value), options, cancellationToken);
        }

        public static T GetValue<T>(this IDistributedCache cache, string name)
        {
            var objectCache = cache.GetString(name);
            if (objectCache is null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(objectCache);
        }

        public static void SetValue(this IDistributedCache cache, string name, object value, double minute)
        {
            var objectCache = cache.GetString(name);
            if (!(objectCache is null))
                cache.Remove(name);

            var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(minute));

            cache.SetString(name, JsonConvert.SerializeObject(value), options);
        }
    }
}