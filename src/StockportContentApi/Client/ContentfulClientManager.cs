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

        public ContentfulClientManager(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Contentful.Core.IContentfulClient GetClient(ContentfulConfig config)
        {
            var client = new Contentful.Core.ContentfulClient(_httpClient, config.AccessKey, config.SpaceKey)
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