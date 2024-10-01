namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupCategoryContentfulFactory : IContentfulFactory<ContentfulGroupCategory, GroupCategory>
{
    public GroupCategory ToModel(ContentfulGroupCategory entry)
    {
        string name = !string.IsNullOrEmpty(entry.Name)
            ? entry.Name
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        string icon = !string.IsNullOrEmpty(entry.Icon)
            ? entry.Icon
            : string.Empty;

        string image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
            entry.Image.File.Url : string.Empty;

        return new GroupCategory(name, slug, icon, image);
    }
}