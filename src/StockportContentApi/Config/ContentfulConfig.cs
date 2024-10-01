namespace StockportContentApi.Config;

public class ContentfulConfig
{
    public readonly string BusinessId;
    private readonly Dictionary<string, string> _config = new();
    public string SpaceKey;
    public string AccessKey;
    public string ManagementKey;
    public string Environment = "master";


    // NEW
    public ContentfulConfig(string spaceKey, string accessKey, string managementKey, string environment = "master")
    {
        SpaceKey = spaceKey;
        AccessKey = accessKey;
        ManagementKey = managementKey;
        Environment = environment;
    }

    // OLD (FUNC)
    public ContentfulConfig(string businessId)
    {
        Utils.Ensure.ArgumentNotNullOrEmpty(businessId, "BUSINESS_ID");
        BusinessId = businessId;
    }

    public Uri ContentfulUrl { get; private set; }
    public Uri ContentfulContentTypesUrl { get; private set; }

    public ContentfulConfig Add(string key, string value)
    {
        _config.Add(key, value);
        return this;
    }

    public ContentfulConfig Build()
    {
        SpaceKey = GetConfigValue($"{BusinessId.ToUpper()}_SPACE");
        AccessKey = GetConfigValue($"{BusinessId.ToUpper()}_ACCESS_KEY");
        ManagementKey = GetConfigValue($"{BusinessId.ToUpper()}_MANAGEMENT_KEY");
        Environment = GetConfigValue($"{BusinessId.ToUpper()}_ENVIRONMENT");

        ContentfulUrl = new Uri($"{GetConfigValue("DELIVERY_URL")}/" +
                                $"spaces/{SpaceKey}/" +
                                $"entries?access_token={AccessKey}");

        ContentfulContentTypesUrl = new Uri($"{GetConfigValue("DELIVERY_URL")}/" +
                                $"spaces/{SpaceKey}/" +
                                $"content_types?access_token={AccessKey}");

        return this;
    }

    private string GetConfigValue(string key)
    {
        if (_config.TryGetValue(key, out string value))
            return value;
            
        throw new ArgumentException($"No value found for '{key}' in the contentful config.");
    }
}