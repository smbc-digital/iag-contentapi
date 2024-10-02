namespace StockportContentApi.ContentfulFactories;

public class ContactUsIdContentfulFactory : IContentfulFactory<ContentfulContactUsId, ContactUsId>
{
    public ContactUsId ToModel(ContentfulContactUsId entry) =>
        new(
            entry.Name,
            entry.Slug,
            entry.EmailAddress,
            entry.SuccessPageButtonText,
            entry.SuccessPageReturnUrl);
}