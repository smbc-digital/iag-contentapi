using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories.GroupFactories
{
    public class GroupCategoryContentfulFactory : IContentfulFactory<ContentfulGroupCategory, GroupCategory>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupCategoryContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public GroupCategory ToModel(ContentfulGroupCategory entry)
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

            var image = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                ? entry.Image.File.Url
                : string.Empty;

            return new GroupCategory(name, slug, icon, image).StripData(_httpContextAccessor);
        }
    }
}