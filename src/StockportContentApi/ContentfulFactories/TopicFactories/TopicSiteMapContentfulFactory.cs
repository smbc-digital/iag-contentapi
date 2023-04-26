namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class TopicSiteMapContentfulFactory : IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>
{
    public TopicSiteMap ToModel(ContentfulTopicForSiteMap entry)
    {
        return new TopicSiteMap(entry.Slug, entry.SunriseDate, entry.SunsetDate);
    }
}