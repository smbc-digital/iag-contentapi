using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class FooterContentfulFactory : IContentfulFactory<ContentfulFooter, Footer>
    {
        private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
        private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FooterContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory, IHttpContextAccessor httpContextAccessor)
        {
            _subitemFactory = subitemFactory;
            _socialMediaFactory = socialMediaFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public Footer ToModel(ContentfulFooter entry)
        {
            var title = !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            var copyrightSection = !string.IsNullOrEmpty(entry.CopyrightSection)
                ? entry.CopyrightSection
                : "";

            var links =
                entry.Links.Where(link => ContentfulHelpers.EntryIsNotALink(link.Sys))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

            var socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                                               .Select(media => _socialMediaFactory.ToModel(media)).ToList();

            return new Footer(title, slug, copyrightSection, links, socialMediaLinks).StripData(_httpContextAccessor);
        }
    }
}