using Contentful.Core.Configuration;

namespace StockportContentApi.Client;

public interface IContentfulClientManager
{
    IContentfulClient GetClient(ContentfulConfig config);
    IContentfulManagementClient GetManagementClient(ContentfulConfig config);
}

public class ContentfulClientManager(System.Net.Http.HttpClient httpClient, IConfiguration configuration) : IContentfulClientManager
{
    private readonly System.Net.Http.HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    public IContentfulClient GetClient(ContentfulConfig config)
    {
        bool.TryParse(_configuration["Contentful:UsePreviewAPI"], out bool usePreviewApi);
        ContentfulOptions options = new()
        {
            SpaceId = config.SpaceKey,
            Environment = config.Environment,
            UsePreviewApi = usePreviewApi,
            DeliveryApiKey = !usePreviewApi ? config.AccessKey : string.Empty,
            PreviewApiKey = usePreviewApi ? config.AccessKey : string.Empty,
            MaxNumberOfRateLimitRetries = 5
        };

        ContentfulClient client = new(_httpClient, options);

        return client;
    }

    public IContentfulManagementClient GetManagementClient(ContentfulConfig config)
        => new ContentfulManagementClient(_httpClient, config.ManagementKey, config.SpaceKey);
}