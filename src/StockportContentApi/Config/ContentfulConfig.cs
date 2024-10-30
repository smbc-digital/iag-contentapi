namespace StockportContentApi.Config;

public interface IContentfulConfigFactory
{
    ContentfulConfig CreateConfig(string businessId);
}

public class ContentfulConfigFactory : IContentfulConfigFactory
{
    public ContentfulConfig CreateConfig(string businessId)
    {
        return new ContentfulConfig(businessId).Build();
    }
}

public class ContentfulConfigProvider
{
    private readonly Dictionary<string, ContentfulConfig> _configurations;

    public ContentfulConfigProvider(IConfiguration configuration)
    {
        _configurations = new Dictionary<string, ContentfulConfig>();

        // Populate configurations for each business
        foreach (var businessId in GetBusinessIds(configuration))
        {
            var config = new ContentfulConfig(businessId)
                .Add("DELIVERY_URL", configuration["Contentful:DeliveryUrl"])
                .Add($"{businessId.ToUpper()}_SPACE", configuration[$"{businessId}:Space"])
                .Add($"{businessId.ToUpper()}_ACCESS_KEY", configuration[$"{businessId}:AccessKey"])
                .Add($"{businessId.ToUpper()}_MANAGEMENT_KEY", configuration[$"{businessId}:ManagementKey"])
                .Add($"{businessId.ToUpper()}_ENVIRONMENT", configuration[$"{businessId}:Environment"])
                .Build();
            
            _configurations[businessId] = config;
        }
    }

    public ContentfulConfig GetConfig(string businessId)
    {
        if (_configurations.TryGetValue(businessId, out var config))
        {
            return config;
        }
        throw new ArgumentException($"No configuration found for business ID '{businessId}'");
    }

    private IEnumerable<string> GetBusinessIds(IConfiguration configuration)
    {
        // Logic to retrieve all business IDs from configuration
        return configuration.GetSection("BusinessIds").Get<string[]>() ?? Array.Empty<string>();
    }
}

[ExcludeFromCodeCoverage]
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