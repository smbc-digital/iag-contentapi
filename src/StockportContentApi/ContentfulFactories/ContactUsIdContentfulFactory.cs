namespace StockportContentApi.ContentfulFactories;

public class ContactUsIdContentfulFactory : IContentfulFactory<ContentfulContactUsId, ContactUsId>
{
    public ContactUsId ToModel(ContentfulContactUsId entry)
    {
        return new ContactUsId(
            entry.Name,
            entry.Slug,
            entry.EmailAddress,
            entry.SuccessPageButtonText,
            entry.SuccessPageReturnUrl);
    }
}