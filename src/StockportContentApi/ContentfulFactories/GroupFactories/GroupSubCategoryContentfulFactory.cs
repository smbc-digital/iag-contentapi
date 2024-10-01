namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupSubCategoryContentfulFactory : IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>
{
    public GroupSubCategory ToModel(ContentfulGroupSubCategory entry)
    {
        string name = !string.IsNullOrEmpty(entry.Name)
            ? entry.Name
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        return new GroupSubCategory(name, slug);
    }
}