using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class CrumbContentfulFactory : IContentfulFactory<ContentfulReference, Crumb>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CrumbContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Crumb ToModel(ContentfulReference entry)
        {
            var title = !string.IsNullOrEmpty(entry.Title) ? entry.Title : entry.Name;

            return new Crumb(title, entry.Slug, entry.Sys.ContentType.SystemProperties.Id).StripData(_httpContextAccessor);
        }
    }
}