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

[ExcludeFromCodeCoverage]
public class Cache : ICache
{
    private readonly ILogger<ICache> _logger;
    private readonly IDistributedCacheWrapper _memoryCache;
    private readonly bool _useLocalCache;
    private readonly bool _useRedisCache;

    public Cache(IDistributedCacheWrapper memoryCache, ILogger<ICache> logger, bool useRedisCache,
        bool useLocalCache = true)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _useRedisCache = useRedisCache;
        _useLocalCache = useLocalCache;
    }

    public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod) =>
        GetFromCacheOrDirectly(cacheKey, fallbackMethod, 60);

    public T GetFromCacheOrDirectly<T>(string cacheKey, Func<T> fallbackMethod, int minutes)
    {
        if (!_useRedisCache && !_useLocalCache || minutes.Equals(0) || TryGetValue(cacheKey, out T result) is false)
        {
            _logger.LogInformation(
                $"CacheWrapper : GetFromCacheOrDirectly<T> : key {cacheKey} not found, getting value for fallback method");

            result = fallbackMethod();

            if (_useRedisCache && minutes > 0 && _memoryCache is not null && result is not null)
                Set(cacheKey, result, minutes);
        }

        return result;
    }

    public async Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod) =>
        await GetFromCacheOrDirectlyAsync(cacheKey, fallbackMethod, 60);

    public async Task<T> GetFromCacheOrDirectlyAsync<T>(string cacheKey, Func<Task<T>> fallbackMethod, int minutes)
    {
        if (!_useRedisCache && !_useLocalCache || minutes.Equals(0) || TryGetValue(cacheKey, out T result) is false)
        {
            _logger.LogInformation(
                $"CacheWrapper : GetFromCacheOrDirectlyAsync<T> : Key '{cacheKey}' not found in cache of type: {typeof(T)}");
            result = await fallbackMethod();

            if ((_useRedisCache || _useLocalCache) && minutes > 0 && _memoryCache is not null && result is not null)
                Set(cacheKey, result, minutes);
        }

        return result;
    }

    public void RemoveItemFromCache(string cacheKey) =>
        _memoryCache.RemoveAsync(cacheKey);

    public void Set(string cacheKey, object cacheEntry, int minutes)
    {
        _logger.LogDebug($"CacheWrapper : Set : Setting key {cacheKey} for {minutes} minutes");

        string data = JsonConvert.SerializeObject(cacheEntry,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        try
        {
            _memoryCache.SetString(cacheKey, data, minutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"CacheWrapper : Set : Error setting key {cacheKey}");
        }
    }

    public bool TryGetValue<T>(object key, out T value)
    {
        value = default;

        if (_memoryCache is null)
        {
            _logger.LogError("CacheWrapper : TryGetValue<T> : Cache is missing");
            return false;
        }

        string returnData;
        try
        {
            returnData = _memoryCache.GetString(key.ToString()).Result;

            if (string.IsNullOrEmpty(returnData))
            {
                _logger.LogDebug(
                    $"CacheWrapper : TryGetValue<T> : data returned for key {key} was either null or empty");

                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(new(), ex,
                "CacheWrapper : TryGetValue<T> : An error occurred trying to read from Redis");

            return false;
        }

        bool output;

        try
        {
            value = JsonConvert.DeserializeObject<T>(returnData);
            _logger.LogDebug($"CacheWrapper : TryGetValue<T> : Key {key} found in cache of type: {typeof(T)}");
            output = value is not null;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(new(), ex,
                $"CacheWrapper : TryGetValue<T> : error deserilizing cached data for key '{key}' with value {returnData}");
            return false;
        }

        return output;
    }
}