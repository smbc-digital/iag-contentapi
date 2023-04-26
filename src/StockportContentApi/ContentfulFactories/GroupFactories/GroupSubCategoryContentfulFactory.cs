namespace StockportContentApi.ContentfulFactories.GroupFactories;

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