using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class NewsRoomContentfulFactory : IContentfulFactory<ContentfulNewsRoom, Newsroom>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewsRoomContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Newsroom ToModel(ContentfulNewsRoom entry)
        {
           return new Newsroom(entry.Alerts, entry.EmailAlerts, entry.EmailAlertsTopicId).StripData(_httpContextAccessor);
        }
    }
}