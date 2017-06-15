using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace StockportContentApi.Utils
{
    public enum CacheDurationMins
    {
        HalfHour = 30,
        Hour = 60,
        TwoHours = 120,
        SixHours = 360,
        TwelveHours = 720,
        Day = 1440
    }

    public interface ICache
    {
        void Set(string cacheKey, object cacheEntry, DistributedCacheEntryOptions cacheEntryOptions);
        bool TryGetValue<T>(object key, out T value);
        T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, int minutes);
        T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod);
        Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod, int minutes);
        Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod);
        void RemoveItemFromCache(string cacheKey);
    }

    public class Cache : ICache
    {
        private IDistributedCacheWrapper _memoryCache;

        public Cache(IDistributedCacheWrapper memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod)
        {
            return GetFromCacheOrDirectly(cacheKey, fallbackMethod, 60);
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, int minutes)
        {
            T result;

            if (TryGetValue(cacheKey, out result) == false)
            {
                result = fallbackMethod();

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(new TimeSpan(minutes * TimeSpan.TicksPerMinute));

                var data = JsonConvert.SerializeObject(result);

                _memoryCache.SetString(cacheKey, data, cacheEntryOptions);
            }

            return result;
        }

        public async Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod)
        {
            return await GetFromCacheOrDirectlyAsync(cacheKey, fallbackMethod, 60);
        }

        public async Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod, int minutes)
        {
            T result;

            if (TryGetValue(cacheKey, out result) == false)
            {
                result = await fallbackMethod();

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(new TimeSpan(minutes * TimeSpan.TicksPerMinute));

                var data = JsonConvert.SerializeObject(result);

                _memoryCache.SetString(cacheKey, data, cacheEntryOptions);
            }

            return result;
        }

        public void RemoveItemFromCache(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public void Set(string cacheKey, object cacheEntry, DistributedCacheEntryOptions cacheEntryOptions)
        {
            var data = JsonConvert.SerializeObject(cacheEntry);

            _memoryCache.SetString(cacheKey, data, cacheEntryOptions);
        }

        public bool TryGetValue<T>(object key, out T value)
        {
            bool output = false;
            value = default(T);
            var returnData = _memoryCache.GetString(key.ToString());

            if (returnData != null)
            {
                value = JsonConvert.DeserializeObject<T>(returnData);
                output = value != null;
            }
            
            return output;
        }
    }
}
