namespace StockportContentApi.ContentfulFactories;

public class ExternalLinkContentfulFactory : IContentfulFactory<ContentfulExternalLink, ExternalLink>
{
    public ExternalLink ToModel(ContentfulExternalLink entry) => 
        new(entry.Title, entry.URL, entry.Teaser);
}