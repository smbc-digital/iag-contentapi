namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class ConverseConfig(string baseUrl, string apiKey)
{
    private readonly Dictionary<string, string> _config = new();
    public string BaseUrl = baseUrl;
    public string ApiKey = apiKey;

    public ConverseConfig() : this(string.Empty, string.Empty)
    { }

    public ConverseConfig Add(string key, string value)
    {
        _config.Add(key, value);
        return this;
    }

    public ConverseConfig Build()
    {
        BaseUrl = GetConfigValue("ConverseApiBaseUrl");
        ApiKey = GetConfigValue("ConverseApiKey");

        return this;
    }

    private string GetConfigValue(string key)
    {
        if (_config.TryGetValue(key, out string value))
            return value;

        throw new ArgumentException($"No value found for '{key}' in the converse config.");
    }
}