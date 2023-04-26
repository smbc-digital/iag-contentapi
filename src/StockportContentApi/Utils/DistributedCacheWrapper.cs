namespace StockportContentApi.Utils;

public interface IDistributedCacheWrapper
{
    Task RemoveAsync(string key, CancellationToken token = default);
    void SetString(string key, string value, int minutes);
    Task<string> GetString(string key, CancellationToken token = default);
}

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheWrapper(IDistributedCache distributedCache) => _distributedCache = distributedCache;

    public async Task<string> GetString(string key, CancellationToken token = default)
        => await _distributedCache.GetStringAsync(key, token);

    public Task RemoveAsync(string key, CancellationToken token = default) => _distributedCache.RemoveAsync(key, token);

    public void SetString(string key, string value, int expiration)
    {
        var distributedCacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(expiration)
        };

        _distributedCache.SetString(key, value, distributedCacheOptions);
    }
}
