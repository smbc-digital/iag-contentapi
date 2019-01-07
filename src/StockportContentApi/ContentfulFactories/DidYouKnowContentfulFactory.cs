using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class DidYouKnowContentfulFactory : IContentfulFactory<ContentfulDidYouKnow, DidYouKnow>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DidYouKnowContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DidYouKnow ToModel(ContentfulDidYouKnow entry)
        {
            return new DidYouKnow(entry.Name, entry.Icon, entry.Text, entry.Link).StripData(_httpContextAccessor);
        }
    }
}
