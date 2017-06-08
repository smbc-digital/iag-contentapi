using System;
using Microsoft.Extensions.Caching.Memory;

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

    public interface ICacheWrapper
    {
        void Set(string cacheKey, object cacheEntry, MemoryCacheEntryOptions cacheEntryOptions);
        bool TryGetValue(object key, out object value);
        T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, long minutes);
        T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod);
        void RemoveItemFromCache(string cacheKey);
    }

    public class CacheWrapper : ICacheWrapper
    {
        private IMemoryCache _memoryCache;

        public CacheWrapper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, long minutes)
        {
            T result;

            if (_memoryCache.TryGetValue(cacheKey, out result) == false)
            {
                result = fallbackMethod();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(new TimeSpan(minutes * TimeSpan.TicksPerMinute));
                _memoryCache.Set(cacheKey, result, cacheEntryOptions);
            }

            return result;
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod)
        {
            return GetFromCacheOrDirectly(cacheKey, fallbackMethod, 12 * 60);
        }

        public void RemoveItemFromCache(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public void Set(string cacheKey, object cacheEntry, MemoryCacheEntryOptions cacheEntryOptions)
        {
            _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
