using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;
using RestSharp.Serializers;

namespace Cortside.RestApiClient {
    public static class IDistributedCacheExtensions {
        public static readonly DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions = new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2),
        };

        public static async Task<T> GetOrSetValueAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> factory, IRestSerializer serializer, DistributedCacheEntryOptions options = null) where T : class {
            var result = await cache.GetValueAsync<T>(key, serializer).ConfigureAwait(false);
            if (result != null) {
                return result;
            }

            result = await factory().ConfigureAwait(false);

            await cache.SetValueAsync(key, result, serializer, options).ConfigureAwait(false);

            return result;
        }

        public static async Task<T> GetValueAsync<T>(this IDistributedCache cache, string key, IRestSerializer serializer) where T : class {
            var data = await cache.GetStringAsync(key).ConfigureAwait(false);
            if (data == null) {
                return default;
            }

            // hack to make it so that IDeserializer and ISerializer interfaces can be used
            var response = new RestResponse();
            PropertyInfo propertyInfo = typeof(RestResponse).GetProperty(nameof(RestResponse.Content));
            propertyInfo.SetValue(response, data);

            return serializer.Deserializer.Deserialize<T>(response);
        }

        public static Task SetValueAsync<T>(this IDistributedCache cache, string key, T value, IRestSerializer serializer, DistributedCacheEntryOptions options = null) where T : class {
            var data = serializer.Serializer.Serialize(value);

            return cache.SetStringAsync(key, data, options ?? DefaultDistributedCacheEntryOptions);
        }
    }
}
