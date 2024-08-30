using Contentful.Core.Configuration;

namespace StockportContentApi.Client;

public interface IContentfulClientManager
{
    IContentfulClient GetClient(ContentfulConfig config);
    IContentfulManagementClient GetManagementClient(ContentfulConfig config);
}

public class ContentfulClientManager : IContentfulClientManager
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ContentfulClientManager(
        System.Net.Http.HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public IContentfulClient GetClient(ContentfulConfig config)
    {
        bool.TryParse(_configuration["Contentful:UsePreviewAPI"], out var usePreviewApi);
        var options = new ContentfulOptions
        {
            SpaceId = config.SpaceKey,
            Environment = config.Environment,
            UsePreviewApi = usePreviewApi,
            DeliveryApiKey = !usePreviewApi ? config.AccessKey : "",
            PreviewApiKey = usePreviewApi ? config.AccessKey : "",
            ResolveEntriesSelectively = true,
            MaxNumberOfRateLimitRetries = 5
        };
        var client = new ContentfulClient(_httpClient, options);        
        return client;
    }

    public IContentfulManagementClient GetManagementClient(ContentfulConfig config)
        => new ContentfulManagementClient(_httpClient, config.ManagementKey, config.SpaceKey);
}