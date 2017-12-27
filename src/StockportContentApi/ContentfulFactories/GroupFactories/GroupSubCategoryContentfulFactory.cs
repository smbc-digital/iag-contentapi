using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories.GroupFactories
{
    public class GroupSubCategoryContentfulFactory : IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupSubCategoryContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public GroupSubCategory ToModel(ContentfulGroupSubCategory entry)
        {
            var name = !string.IsNullOrEmpty(entry.Name)
                ? entry.Name
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            return new GroupSubCategory(name, slug).StripData(_httpContextAccessor);
        }
    }
}