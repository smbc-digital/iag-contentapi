namespace StockportContentApi.ContentfulFactories;

public class FilterContentfulFactory : IContentfulFactory<ContentfulFilter, Filter>
{
    public Filter ToModel(ContentfulFilter entry)
        => entry is null ? null : new Filter(entry);
}