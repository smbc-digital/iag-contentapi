using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class FooterFactory : IFactory<Footer>
    {
        private readonly IBuildContentTypesFromReferences<SubItem> _subItemListFactory;
        private readonly IBuildContentTypesFromReferences<SocialMediaLink> _socialMediaLinkListFactory;

        public FooterFactory(IBuildContentTypesFromReferences<SubItem> subItemListFactory, IBuildContentTypesFromReferences<SocialMediaLink> socialMediaLinkListFactory)
        {
            _subItemListFactory = subItemListFactory;
            _socialMediaLinkListFactory = socialMediaLinkListFactory;
        }

        public Footer Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            dynamic fields = entry.fields;
            var slug = (string)fields.slug ?? string.Empty;
            var title = (string)fields.title ?? string.Empty;
            var copyrightSection = (string)fields.copyrightSection ?? string.Empty;
            var links = _subItemListFactory.BuildFromReferences(fields.links, contentfulResponse);
            var socialMediaLinks = _socialMediaLinkListFactory.BuildFromReferences(fields.socialMediaLinks, contentfulResponse);

            return new Footer(title, slug, copyrightSection, links, socialMediaLinks);
        }
    }
}
