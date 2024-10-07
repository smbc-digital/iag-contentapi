namespace StockportContentApi.ContentfulFactories;

public class HeaderContentfulFactory : IContentfulFactory<ContentfulHeader, Header>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;

    public HeaderContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory)
    {
        _subitemFactory = subitemFactory;
    }

    public Header ToModel(ContentfulHeader entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : string.Empty;

        List<SubItem> items = entry.Items.Where(link => ContentfulHelpers.EntryIsNotALink(link.Sys))
                                .Select(item => _subitemFactory.ToModel(item)).ToList();

        string logo = string.IsNullOrEmpty(entry.Logo.File.Url) ? string.Empty : entry.Logo.File.Url;

        return new Header(title, items, logo);
    }
}