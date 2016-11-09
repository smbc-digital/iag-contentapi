using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class TopicRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly string _contentType = "topic";
        private readonly IFactory<Topic> _factory;
        private readonly ContentfulClient _contentfulClient;

        public TopicRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Topic> factory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _factory = factory;
            _contentfulApiUrl = config.ContentfulUrl.ToString();
        }

        public async Task<HttpResponse> GetTopicByTopicSlug(string topicSlug)
        {
            var referenceLevelLimit = 1;
            var contentfulResponse = await _contentfulClient.Get(UrlFor(_contentType, referenceLevelLimit, topicSlug));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{topicSlug}'");

            var topic = _factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            return topic == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{topicSlug}'")
                : HttpResponse.Successful(topic);
        }

        //TODO: extract out to its own class ContentfulUrlBuilder [Tech-time]
        // + single responsibility for building urls for contentful
        // + easier to test it out and use it in the test
        // + single source of truth for building contentful urls and query
        // + one place to change the url and query
        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}"; 
        }
    }
}