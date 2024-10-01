namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class ProfileParentTopicContentfulFactory : IContentfulFactory<ContentfulProfile, Topic>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
    private readonly DateComparer _dateComparer;

    public ProfileParentTopicContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        ITimeProvider timeProvider)
    {
        _subItemFactory = subItemFactory;
        _dateComparer = new DateComparer(timeProvider);
    }

    private ContentfulProfile _entry;

    public Topic ToModel(ContentfulProfile entry)
    {
        _entry = entry;

        if (_entry is null)
            return new NullTopic();

        ContentfulReference topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(o => o.Sys.ContentType.SystemProperties.Id.Equals("topic"));

        if (topicInBreadcrumb is null) return new NullTopic();

        List<SubItem> subItems = topicInBreadcrumb.SubItems
            .Select(CheckCurrentProfile)
            .Where(subItem => subItem is not null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        List<SubItem> secondaryItems = topicInBreadcrumb.SecondaryItems
            .Select(CheckCurrentProfile)
            .Where(subItem => subItem is not null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        return new Topic(topicInBreadcrumb.Name, topicInBreadcrumb.Slug, subItems, secondaryItems);
    }

    private ContentfulReference CheckCurrentProfile(ContentfulReference item)
    {
        if (!item.Sys.Id.Equals(_entry.Sys.Id)) return item;

        return new ContentfulReference
        {
            Icon = _entry.Icon,
            Title = _entry.Title,
            SunriseDate = _entry.SunriseDate,
            SunsetDate = _entry.SunsetDate,
            Slug = _entry.Slug,
            Image = _entry.Image,
            Teaser = _entry.Teaser,
            Sys = { ContentType = new ContentType() { SystemProperties = new SystemProperties() { Id = "profile" } } }
        };
    }
}