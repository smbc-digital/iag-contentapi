using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class AlertContentfulFactory : IContentfulFactory<ContentfulAlert, Alert>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AlertContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Alert ToModel(ContentfulAlert entry)
        {
            return new Alert(entry.Title, entry.SubHeading, entry.Body, entry.Severity, entry.SunriseDate, entry.SunsetDate,entry.Slug).StripData(_httpContextAccessor);
        }
    }
}