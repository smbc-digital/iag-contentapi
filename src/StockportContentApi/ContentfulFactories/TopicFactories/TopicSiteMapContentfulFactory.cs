namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class TopicSiteMapContentfulFactory : IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>
{
    public TopicSiteMap ToModel(ContentfulTopicForSiteMap entry)
        => new(entry.Slug, entry.SunriseDate, entry.SunsetDate);
}