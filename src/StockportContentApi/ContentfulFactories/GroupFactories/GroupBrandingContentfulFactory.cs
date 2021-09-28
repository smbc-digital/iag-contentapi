using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories.GroupFactories
{
    public class GroupBrandingContentfulFactory : IContentfulFactory<ContentfulGroupBranding, GroupBranding>
    {
        public GroupBranding ToModel(ContentfulGroupBranding entry)
        {
            var file = new MediaAsset();

            if (entry != null && entry.File != null && entry.File.File != null)
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
}
