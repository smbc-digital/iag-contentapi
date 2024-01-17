﻿namespace StockportContentApi.Utils;

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
    void Set(string cacheKey, object cacheEntry, int minutes);
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

        if (!_useRedisCache || minutes == 0 || TryGetValue(cacheKey, out result) == false)
        {
            _logger.LogInformation($"CacheWrapper : GetFromCacheOrDirectly<T> : key {cacheKey} not found, getting value for fallback method");

            result = fallbackMethod();

            if (_useRedisCache && minutes > 0 && _memoryCache != null && result != null)
            {
                Set(cacheKey, result, minutes);
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

        if (!_useRedisCache || minutes == 0 || TryGetValue(cacheKey, out result) == false)
        {
            _logger.LogInformation($"CacheWrapper : GetFromCacheOrDirectlyAsync<T> : Key '{cacheKey}' not found in cache of type: {typeof(T)}");
            result = await fallbackMethod();

            if (_useRedisCache && minutes > 0 && _memoryCache != null && result != null)
            {
                Set(cacheKey, result, minutes);
            }
        }

        return result;
    }

    public void RemoveItemFromCache(string cacheKey)
    {
        _memoryCache.RemoveAsync(cacheKey);
    }

    public void Set(string cacheKey, object cacheEntry, int minutes)
    {
        _logger.LogInformation($"CacheWrapper : Set : Setting key {cacheKey} for {minutes} minutes");
        
        var data = JsonConvert.SerializeObject(cacheEntry, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        try{
            _memoryCache.SetString(cacheKey, data, minutes);
        }
        catch(Exception ex){
            _logger.LogError(ex, $"CacheWrapper : Set : Error setting key {cacheKey}");
        }
    }

    public bool TryGetValue<T>(object key, out T value)
    {
        bool output = false;
        value = default(T);

        if (_memoryCache == null)
        {
            _logger.LogWarning("CacheWrapper : TryGetValue<T> : Cache is missing");
            return false;
        }

        var returnData = string.Empty;

        try
        {
            returnData = _memoryCache.GetString(key.ToString()).Result;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(new EventId(), ex, "CacheWrapper : TryGetValue<T> : An error occurred trying to read from Redis");
            return false;
        }

        if (string.IsNullOrEmpty(returnData))
        {
            _logger.LogWarning($"CacheWrapper : TryGetValue<T> : data returned for key {key} was either null or empty");
            return false;
        }

        if (returnData != null)
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(returnData);
                _logger.LogInformation($"CacheWrapper : TryGetValue<T> : Key {key} found in cache of type: {typeof(T)}");
                output = value != null;
            }
            catch(Exception ex)
            {
                _logger.LogCritical(new EventId(), ex, $"CacheWrapper : TryGetValue<T> : error deserilizing cached data for key '{key}' with value {returnData}");
                return false;
            }
        }

        return output;
    }
}
