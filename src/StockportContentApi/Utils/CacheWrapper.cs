using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using BinaryFormatter;
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

    public interface ICacheWrapper
    {
        void Set(string cacheKey, object cacheEntry, DistributedCacheEntryOptions cacheEntryOptions);
        bool TryGetValue<T>(object key, out T value);
        T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, long minutes);
        T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod);
        void RemoveItemFromCache(string cacheKey);
        Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod);
    }

    public class CacheWrapper : ICacheWrapper
    {
        private IDistributedCache _memoryCache;

        public CacheWrapper(IDistributedCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, long minutes)
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
            T result;

            if (TryGetValue(cacheKey, out result) == false)
            {
                result = await fallbackMethod();

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(new TimeSpan(12 * TimeSpan.TicksPerMinute));

                var data = JsonConvert.SerializeObject(result);

                //var binFormatter = new BinaryConverter();
                //var mStream = new MemoryStream();
                //binFormatter.Serialize(result);
                //var data = mStream.ToArray();

                _memoryCache.SetString(cacheKey, data, cacheEntryOptions);
            }

            return result;
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod)
        {
            return GetFromCacheOrDirectly(cacheKey, fallbackMethod, 60);
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
