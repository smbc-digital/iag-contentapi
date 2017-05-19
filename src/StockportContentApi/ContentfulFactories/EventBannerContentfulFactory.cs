using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class EventBannerContentfulFactory : IContentfulFactory<Entry<ContentfulEventBanner>, EventBanner>
    {
        public EventBanner ToModel(Entry<ContentfulEventBanner> entry)
        {
            return new EventBanner(entry.Fields.Title, entry.Fields.Teaser, entry.Fields.Icon, entry.Fields.Link);
        }
    }
}
