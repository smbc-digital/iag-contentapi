namespace StockportContentApi.ContentfulFactories;

public class PublicationsTemplateContentfulFactory() : IContentfulFactory<ContentfulPublicationsTemplate, PublicationsTemplate>
{
    public PublicationsTemplate ToModel(ContentfulPublicationsTemplate entry)
    {
        if (entry is null)
            return null;

        return new PublicationsTemplate
        {
            Slug = entry.Slug,
            Title = entry.Title,
            MetaDescription = entry.MetaDescription,
            Body = entry.Body
        };
    }
}