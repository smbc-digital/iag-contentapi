using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class ContactUsCategoryContentfulFactory : IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUsCategoryContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ContactUsCategory ToModel(ContentfulContactUsCategory entry)
        {
            return new ContactUsCategory(entry.Title, entry.BodyTextLeft, entry.BodyTextRight, entry.Icon).StripData(_httpContextAccessor);
        }
    }
}