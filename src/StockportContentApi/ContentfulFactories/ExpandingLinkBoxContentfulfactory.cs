namespace StockportContentApi.ContentfulFactories;

public class ExpandingLinkBoxContentfulfactory : IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>
{
    private IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly DateComparer _dateComparer;

    public ExpandingLinkBoxContentfulfactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, ITimeProvider timeProvider)
    {
        _subitemFactory = subitemFactory;
        _dateComparer = new DateComparer(timeProvider);
    }

    public ExpandingLinkBox ToModel(ContentfulExpandingLinkBox entry)
    {
        return new ExpandingLinkBox(entry.Title,
            entry.Links.Where(link => ContentfulHelpers.EntryIsNotALink(link.Sys)
                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(link.SunriseDate, link.SunsetDate))
                                     .Select(e => _subitemFactory.ToModel(e)).ToList());
    }
}

