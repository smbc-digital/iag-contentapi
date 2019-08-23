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
            string image;
            string imageDescription;

            if(entry.File != null)
            {
                image = entry.File.File.Url != null ? entry.File.File.Url : string.Empty;
                imageDescription = entry.File.Description != null ? entry.File.Description : string.Empty;
            }
            else
            {
                image = string.Empty;
                imageDescription = string.Empty;
            }

            return new GroupBranding(entry.Title, entry.Text, image, imageDescription, entry.Url).StripData(_httpContextAccessor);
        }
    }
}
