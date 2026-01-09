namespace StockportContentApi.ContentfulFactories;

public class PublicationSectionContentfulFactory() : IContentfulFactory<ContentfulPublicationSection, PublicationSection>
{
    public PublicationSection ToModel(ContentfulPublicationSection entry)
    {
        if (entry is null)
            return null;

        return new PublicationSection
        {
            Title = entry.Title,
            Slug = entry.Slug,
            MetaDescription = entry.MetaDescription,
            Body = entry.Body
        };
    }
}