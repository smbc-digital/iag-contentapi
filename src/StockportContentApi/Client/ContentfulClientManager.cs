using StockportContentApi.Config;

namespace StockportContentApi.Client
{
    public interface IContentfulClientManager
    {
        Contentful.Core.IContentfulClient GetClient(ContentfulConfig config);
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
            return new Contentful.Core.ContentfulClient(_httpClient, config.AccessKey, config.SpaceKey);
        }
    }
}