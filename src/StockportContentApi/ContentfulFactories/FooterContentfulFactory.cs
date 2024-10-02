namespace StockportContentApi.ContentfulFactories;

public class FooterContentfulFactory : IContentfulFactory<ContentfulFooter, Footer>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;

    public FooterContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory)
    {
        _subitemFactory = subitemFactory;
        _socialMediaFactory = socialMediaFactory;
    }

    public Footer ToModel(ContentfulFooter entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        string copyrightSection = !string.IsNullOrEmpty(entry.CopyrightSection)
            ? entry.CopyrightSection
            : string.Empty;

        List<SubItem> links = entry.Links.Where(link => ContentfulHelpers.EntryIsNotALink(link.Sys))
                                .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<SocialMediaLink> socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                                                    .Select(media => _socialMediaFactory.ToModel(media)).ToList();

        return new Footer(title, slug, links, socialMediaLinks);
    }
}