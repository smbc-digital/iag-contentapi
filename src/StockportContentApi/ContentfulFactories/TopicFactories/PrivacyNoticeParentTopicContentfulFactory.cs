namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class PrivacyNoticeParentTopicContentfulFactory : IContentfulFactory<ContentfulPrivacyNotice, Topic>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
    private readonly DateComparer _dateComparer;

    public PrivacyNoticeParentTopicContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        ITimeProvider timeProvider)
    {
        _subItemFactory = subItemFactory;
        _dateComparer = new DateComparer(timeProvider);
    }

    private ContentfulPrivacyNotice _entry;

    public Topic ToModel(ContentfulPrivacyNotice entry)
    {
        _entry = entry;
        if (_entry is null)
            return new NullTopic();

        ContentfulReference topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(_ => _.Sys.ContentType.SystemProperties.Id.Equals("topic"));

        if (topicInBreadcrumb is null) return new NullTopic();

        List<SubItem> subItems = topicInBreadcrumb.SubItems
            .Select(CheckCurrentPrivacyNotice)
            .Where(subItem => subItem is not null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        List<SubItem> secondaryItems = topicInBreadcrumb.SecondaryItems
            .Select(CheckCurrentPrivacyNotice)
            .Where(subItem => subItem is not null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        return new Topic(topicInBreadcrumb.Name, topicInBreadcrumb.Slug, subItems, secondaryItems);
    }

    private ContentfulReference CheckCurrentPrivacyNotice(ContentfulReference item)
    {
        if (!item.Sys.Id.Equals(_entry.Sys.Id)) return item;

        // the link is to the current article
        return new ContentfulReference
        {
            Icon = _entry.Icon,
            Title = _entry.Title,
            SunriseDate = _entry.SunriseDate,
            SunsetDate = _entry.SunsetDate,
            Slug = _entry.Slug,
            Image = _entry.Image,
            Teaser = _entry.Teaser,
            Sys = { ContentType = new ContentType() { SystemProperties = new SystemProperties() { Id = "privacy-notice" } } }
        };
    }
}