using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories.EventFactories
{
    public class EventBannerContentfulFactory : IContentfulFactory<ContentfulEventBanner, EventBanner>
    {
        public EventBanner ToModel(ContentfulEventBanner entry)
        {
            return new EventBanner(entry.Title, entry.Teaser, entry.Icon, entry.Link);
        }
    }
}
