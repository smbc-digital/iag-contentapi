namespace StockportContentApi.ContentfulFactories;

public class CrumbContentfulFactory : IContentfulFactory<ContentfulReference, Crumb>
{
    public Crumb ToModel(ContentfulReference entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : entry.Name;

        return new Crumb(title, entry.Slug, entry.Sys.ContentType.SystemProperties.Id, entry.Websites);
    }
}