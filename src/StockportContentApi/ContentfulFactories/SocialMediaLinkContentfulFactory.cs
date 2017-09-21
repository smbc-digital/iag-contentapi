using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class SocialMediaLinkContentfulFactory : IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SocialMediaLinkContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public SocialMediaLink ToModel(ContentfulSocialMediaLink link)
        {
            return new SocialMediaLink(link.Title, link.Slug, link.Url, link.Icon).StripData(_httpContextAccessor);
        }
    }
}