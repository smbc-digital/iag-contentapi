namespace StockportContentApi.ContentfulFactories.EventFactories;

public class EventBannerContentfulFactory : IContentfulFactory<ContentfulEventBanner, EventBanner>
{
    public EventBanner ToModel(ContentfulEventBanner entry)
        => new(entry.Title, entry.Teaser, entry.Icon, entry.Link, entry.Colour);
}