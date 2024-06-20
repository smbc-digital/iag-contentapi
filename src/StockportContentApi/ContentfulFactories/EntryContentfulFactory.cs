namespace StockportContentApi.ContentfulFactories;

public class EntryContentfulFactory : IContentfulFactory<ContentfulEntry, Entry>
{
    public EntryContentfulFactory()
    {
    }

    public Entry ToModel(ContentfulEntry entry)
    {
        
        return new Entry
        {
            Content = entry.Content
        };
    }
}