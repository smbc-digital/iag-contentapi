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
            var file = new MediaAsset();

            if (entry != null && entry.File != null && entry.File.Url != null)
            {
                file = new MediaAsset
                {
                    Url = entry.File.Url,
                    Description = entry.File.Description
                };
            }

            return new GroupBranding(entry.Title, entry.Text, file, entry.Url).StripData(_httpContextAccessor);
        }
    }
}
