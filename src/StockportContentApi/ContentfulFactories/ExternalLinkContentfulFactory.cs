namespace StockportContentApi.ContentfulFactories;

public class ExternalLinkContentfulFactory : IContentfulFactory<ContentfulExternalLink, ExternalLink>
{
    public ExternalLink ToModel(ContentfulExternalLink entry)
    {
        return new ExternalLink(entry.Title, entry.URL, entry.Teaser);
    }
}