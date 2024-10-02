namespace StockportContentApi.ContentfulFactories;

public class AtoZContentfulFactory : IContentfulFactory<ContentfulAtoZ, AtoZ>
{
    public AtoZ ToModel(ContentfulAtoZ entry)
    {
        string title = string.IsNullOrEmpty(entry.Title)
            ? entry.Name 
            : entry.Title;
            
        string type = entry.Sys.ContentType.SystemProperties.Id ?? string.Empty;

        return new AtoZ(title, entry.Slug, entry.Teaser, type, entry.AlternativeTitles);
    }
}