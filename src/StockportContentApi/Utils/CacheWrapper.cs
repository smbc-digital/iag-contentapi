﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockportContentApi.Repositories;

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
        private readonly IDistributedCacheWrapper _memoryCache;
        private readonly ILogger<ICache> _logger;
        private readonly bool _useRedisCache;

        public Cache(IDistributedCacheWrapper memoryCache, ILogger<ICache> logger, bool useRedisCache)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _useRedisCache = useRedisCache;
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod)
        {
            return GetFromCacheOrDirectly(cacheKey, fallbackMethod, 60);
        }

        public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, int minutes)
        {
            T result;

            if (!_useRedisCache || TryGetValue(cacheKey, out result) == false)
            {
                result = fallbackMethod();

                if (_useRedisCache)
                {
                    var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(minutes * TimeSpan.TicksPerMinute));

                    var data = JsonConvert.SerializeObject(result);

                    _memoryCache.SetString(cacheKey, data, cacheEntryOptions);
                }
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

            if (!_useRedisCache || TryGetValue(cacheKey, out result) == false)
            {
                //_logger.LogInformation("Key not found in cache:" + cacheKey + " of type:" + typeof(T));
                result = await fallbackMethod();

                if (_useRedisCache && _memoryCache != null)
                {
                    var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(minutes * TimeSpan.TicksPerMinute));

                    var data = JsonConvert.SerializeObject(result);

                    try
                    {
                        _memoryCache.SetString(cacheKey, data, cacheEntryOptions);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(new EventId(), ex, "An error occurred trying to write to Redis");
                    }
                }
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

            if (_memoryCache == null)
            {
                _logger.LogInformation("Cache is missing");
                return false;
            }

            var returnData = string.Empty;
            
            try
            {
                returnData = _memoryCache.GetString(key.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogCritical(new EventId(), ex, "An error occurred trying to read from Redis");
                return false;
            }

            if (returnData != null)
            {
                value = JsonConvert.DeserializeObject<T>(returnData);
                //_logger.LogInformation("Key found in cache:" + key + " of type:" + typeof(T));
                output = value != null;
            }
            
            return output;
        }
    }
}
