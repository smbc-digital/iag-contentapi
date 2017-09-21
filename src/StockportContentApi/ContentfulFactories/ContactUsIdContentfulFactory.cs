using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ContactUsIdContentfulFactory : IContentfulFactory<ContentfulContactUsId, ContactUsId>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUsIdContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ContactUsId ToModel(ContentfulContactUsId entry)
        {
            return new ContactUsId(
                entry.Name,
                entry.Slug,
                entry.EmailAddress).StripData(_httpContextAccessor);
        }
    }
}