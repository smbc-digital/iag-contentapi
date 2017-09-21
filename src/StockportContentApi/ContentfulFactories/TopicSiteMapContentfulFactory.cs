using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class TopicSiteMapContentfulFactory : IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TopicSiteMapContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public TopicSiteMap ToModel(ContentfulTopicForSiteMap entry)
        {
            return new TopicSiteMap(entry.Slug, entry.SunriseDate, entry.SunsetDate).StripData(_httpContextAccessor);
        }
    }
}