namespace StockportContentApi.ContentfulFactories;

public class ContactUsCategoryContentfulFactory : IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>
{
    public ContactUsCategory ToModel(ContentfulContactUsCategory entry) =>
        new(entry.Title, entry.BodyTextLeft, entry.BodyTextRight, entry.Icon);
}