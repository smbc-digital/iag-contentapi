using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class EventCategoryContentfulFactory : IContentfulFactory<ContentfulEventCategory, EventCategory>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventCategoryContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public EventCategory ToModel(ContentfulEventCategory entry)
        {
            var name = !string.IsNullOrEmpty(entry.Name)
                ? entry.Name
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            var icon = !string.IsNullOrEmpty(entry.Icon)
                ? entry.Icon
                : "";

            return new EventCategory(name, slug, icon).StripData(_httpContextAccessor);
        }
    }
}