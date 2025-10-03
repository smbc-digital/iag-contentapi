namespace StockportContentApi.ContentfulFactories;

public class FooterContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
                                    IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory) : IContentfulFactory<ContentfulFooter, Footer>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory = subitemFactory;
    private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory = socialMediaFactory;

    public Footer ToModel(ContentfulFooter entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        string footerContent1 = !string.IsNullOrEmpty(entry.FooterContent1)
            ? entry.FooterContent1
            : string.Empty;

        string footerContent2 = !string.IsNullOrEmpty(entry.FooterContent2)
            ? entry.FooterContent2
            : string.Empty;

        string footerContent3 = !string.IsNullOrEmpty(entry.FooterContent3)
            ? entry.FooterContent3
            : string.Empty;

        List<SubItem> links = entry.Links
                                .Where(link => ContentfulHelpers.EntryIsNotALink(link.Sys))
                                .Select(_subitemFactory.ToModel).ToList();

        List<SocialMediaLink> socialMediaLinks = entry.SocialMediaLinks
                                                    .Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                                                    .Select(_socialMediaFactory.ToModel).ToList();

        return new Footer(title, slug, links, socialMediaLinks, footerContent1, footerContent2, footerContent3);
    }
}