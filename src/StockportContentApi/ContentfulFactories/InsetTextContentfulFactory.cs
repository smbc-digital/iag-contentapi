using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class InsetTextContentfulFactory : IContentfulFactory<ContentfulInsetText, InsetText>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InsetTextContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public InsetText ToModel(ContentfulInsetText entry)
        {
            return new InsetText(entry.Title, entry.SubHeading, entry.Body, entry.Colour, entry.Slug).StripData(_httpContextAccessor);
        }
    }
}