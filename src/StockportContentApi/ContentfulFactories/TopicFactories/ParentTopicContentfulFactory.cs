namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class ParentTopicContentfulFactory : IContentfulFactory<ContentfulArticle, Topic>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
    private readonly DateComparer _dateComparer;

    public ParentTopicContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        ITimeProvider timeProvider)
    {
        _subItemFactory = subItemFactory;
        _dateComparer = new DateComparer(timeProvider);
    }

    private ContentfulArticle _entry;

    public Topic ToModel(ContentfulArticle entry)
    {
        _entry = entry;

        var topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(o => o.Sys.ContentType.SystemProperties.Id == "topic");

        if (topicInBreadcrumb == null) return new NullTopic();

        var subItems = topicInBreadcrumb.SubItems
            .Select(CheckCurrentArticle)
            .Where(subItem => subItem != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        var secondaryItems = topicInBreadcrumb.SecondaryItems
            .Select(CheckCurrentArticle)
            .Where(subItem => subItem != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        return new Topic(topicInBreadcrumb.Name, topicInBreadcrumb.Slug, subItems, secondaryItems);
    }

    private ContentfulReference CheckCurrentArticle(ContentfulReference item)
    {
        if (item.Sys.Id != _entry.Sys.Id) return item;

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
            Sys = { ContentType = new ContentType() { SystemProperties = new SystemProperties() { Id = "article" } } }
        };
    }
}