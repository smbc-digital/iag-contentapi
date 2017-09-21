using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class AtoZContentfulFactory : IContentfulFactory<ContentfulAtoZ, AtoZ>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AtoZContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public AtoZ ToModel(ContentfulAtoZ entry)
        {
            var title = string.IsNullOrEmpty(entry.Title) ? (string) entry.Name : (string) entry.Title;
            var type = entry.Sys.ContentType.SystemProperties.Id ?? string.Empty;
            return new AtoZ(title, entry.Slug, entry.Teaser, type, entry.AlternativeTitles).StripData(_httpContextAccessor);
        }
    }
}