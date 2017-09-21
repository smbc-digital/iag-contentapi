using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class KeyFactContentfulFactory : IContentfulFactory<ContentfulKeyFact, KeyFact>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KeyFactContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public KeyFact ToModel(ContentfulKeyFact entry)
        {
            return new KeyFact(entry.Icon, entry.Text, entry.Link).StripData(_httpContextAccessor);
        }
    }
}