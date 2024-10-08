namespace StockportContentApi.ContentfulFactories;

public class SiteHeaderContentfulFactory : IContentfulFactory<ContentfulSiteHeader, SiteHeader>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;

    public SiteHeaderContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory) =>
        _subitemFactory = subitemFactory;

    public SiteHeader ToModel(ContentfulSiteHeader entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : string.Empty;

        List<SubItem> items = entry.Items
            .Where(link => ContentfulHelpers.EntryIsNotALink(link.Sys))
            .Select(item => _subitemFactory.ToModel(item))
            .ToList();

        string logo = string.IsNullOrEmpty(entry.Logo.File.Url) ? string.Empty : entry.Logo.File.Url;

        return new SiteHeader(title, items, logo);
    }
}