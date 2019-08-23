using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories.GroupFactories
{
    public class GroupBrandingContentfulFactory : IContentfulFactory<ContentfulGroupBranding, GroupBranding>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupBrandingContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public GroupBranding ToModel(ContentfulGroupBranding entry)
        {
            //var image = new MediaAsset()
            //{
            //    Url = ContentfulHelpers.EntryIsNotALink(entry.File.SystemProperties)
            //                               ? entry.File.File.Url : string.Empty,
            //    Description = entry.File.Description != null ? entry.File.Description : string.Empty
            //};

            var image = new MediaAsset();

            //var image = entry.File != null ? entry.File.File.Url : string.Empty;

            return new GroupBranding(entry.Title, entry.Text, image, entry.Url).StripData(_httpContextAccessor);
        }
    }
}
