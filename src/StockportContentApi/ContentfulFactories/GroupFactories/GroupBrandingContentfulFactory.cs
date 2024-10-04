namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupBrandingContentfulFactory : IContentfulFactory<ContentfulGroupBranding, GroupBranding>
{
    public GroupBranding ToModel(ContentfulGroupBranding entry)
    {
        MediaAsset file = new();

        if (entry is not null && entry.File is not null && entry.File.File is not null)
        {
            file = new MediaAsset
            {
                Url = entry.File.File.Url,
                Description = entry.File.Description
            };
        }

        return new GroupBranding(entry.Title, entry.Text, file, entry.Url);
    }
}
