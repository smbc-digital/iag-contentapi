using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class ContactUsCategoryContentfulFactory : IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>
    {
        public ContactUsCategory ToModel(ContentfulContactUsCategory entry)
        {
            return new ContactUsCategory(entry.Title, entry.BodyTextLeft, entry.BodyTextRight, entry.Icon);
        }
    }
}