namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class CacheKeyConfig
{
    public readonly string BusinessId;
    private readonly Dictionary<string, string> _config = new();
    public string EventsCacheKey;
    public string NewsCacheKey;

    public CacheKeyConfig(string eventsCacheKey, string newsCacheKey)
    {
        EventsCacheKey = eventsCacheKey;
        NewsCacheKey = newsCacheKey;
    }

    public CacheKeyConfig(string businessId)
    {
        Utils.Ensure.ArgumentNotNullOrEmpty(businessId, "BUSINESS_ID");
        BusinessId = businessId;
    }

    public CacheKeyConfig Add(string key, string value)
    {
        _config.Add(key, value);
        return this;
    }

    public CacheKeyConfig Build()
    {
        EventsCacheKey = GetConfigValue($"{BusinessId.ToUpper()}_EventsCacheKey");
        NewsCacheKey = GetConfigValue($"{BusinessId.ToUpper()}_NewsCacheKey");

        return this;
    }

    private string GetConfigValue(string key)
    {
        if (_config.TryGetValue(key, out string value))
            return value;

        throw new ArgumentException($"No value found for '{key}' in the contentful config.");
    }
}