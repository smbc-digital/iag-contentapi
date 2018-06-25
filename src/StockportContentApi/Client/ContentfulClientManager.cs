using Contentful.Core;
using Microsoft.Extensions.Configuration;
using StockportContentApi.Config;

namespace StockportContentApi.Client
{
    public interface IContentfulClientManager
    {
        IContentfulClient GetClient(ContentfulConfig config);
        IContentfulManagementClient GetManagementClient(ContentfulConfig config);
    }

    public class ContentfulClientManager : IContentfulClientManager
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        private IConfiguration _configuration;

        public ContentfulClientManager(System.Net.Http.HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IContentfulClient GetClient(ContentfulConfig config)
        {
            bool.TryParse(_configuration["Contentful:UsePreviewAPI"], out var usePreviewApi);
            var client = new ContentfulClient(_httpClient, !usePreviewApi ? config.AccessKey : "", usePreviewApi ? config.AccessKey : "", config.SpaceKey)
            {
                ResolveEntriesSelectively = true
            };

            return client;
        }
            
        public IContentfulManagementClient GetManagementClient(ContentfulConfig config)
        {
            var client = new ContentfulManagementClient(_httpClient, config.ManagementKey, config.SpaceKey);
            return client;
        }
    }
}