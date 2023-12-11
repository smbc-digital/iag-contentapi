namespace StockportContentApi.ContentfulFactories
{
    public class FilterContentfulFactory : IContentfulFactory<ContentfulFilter, Filter>
    {
        public Filter ToModel(ContentfulFilter entry)
        {
            if (entry is null)
                return null;

            return new Filter(entry);
        }
    }
}
