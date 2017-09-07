using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupSubCategoryContentfulFactory : IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>
    {
        public GroupSubCategory ToModel(ContentfulGroupSubCategory entry)
        {
            var name = !string.IsNullOrEmpty(entry.Name)
                ? entry.Name
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            return new GroupSubCategory(name, slug);
        }
    }
}