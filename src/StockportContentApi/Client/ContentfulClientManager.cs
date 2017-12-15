using Microsoft.Extensions.Configuration;
using StockportContentApi.Config;

namespace StockportContentApi.Client
{
    public interface IContentfulClientManager
    {
        Contentful.Core.IContentfulClient GetClient(ContentfulConfig config);
        Contentful.Core.IContentfulManagementClient GetManagementClient(ContentfulConfig config);
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

        public Contentful.Core.IContentfulClient GetClient(ContentfulConfig config)
        {
            bool.TryParse(_configuration["Contentful:UsePreviewAPI"], out var usePreviewApi);
            var client = new Contentful.Core.ContentfulClient(_httpClient, config.AccessKey, config.SpaceKey, usePreviewApi)
            {
                ResolveEntriesSelectively = true
            };
            return client;
        }

        public Contentful.Core.IContentfulManagementClient GetManagementClient(ContentfulConfig config)
        {
            var client = new Contentful.Core.ContentfulManagementClient(_httpClient, config.ManagementKey, config.SpaceKey);
            return client;
        }
    }
}