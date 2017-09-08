using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupHomepageContentfulFactory : IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>
    {

        public GroupHomepage ToModel(ContentfulGroupHomepage entry)
        {
            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                           ? entry.BackgroundImage.File.Url : string.Empty;

            return new GroupHomepage(entry.Title, entry.Slug, backgroundImage);
        }
    }
}
