namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class ParentTopicContentfulFactory : IContentfulFactory<ContentfulArticle, Topic>
{
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
    private ContentfulArticle _entry;

    public ParentTopicContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        ITimeProvider timeProvider)
    {
        _subItemFactory = subItemFactory;
        _dateComparer = new(timeProvider);
    }

    public Topic ToModel(ContentfulArticle entry)
    {
        _entry = entry;

        ContentfulReference topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(o => o.Sys.ContentType.SystemProperties.Id.Equals("topic"));

        if (topicInBreadcrumb is null)
            return new NullTopic();

        List<SubItem> subItems = topicInBreadcrumb.SubItems.Select(CheckCurrentArticle)
                                    .Where(subItem => subItem is not null 
                                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                    .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        List<SubItem> secondaryItems = topicInBreadcrumb.SecondaryItems.Select(CheckCurrentArticle)
                                        .Where(subItem => subItem is not null
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        return new(topicInBreadcrumb.Name, topicInBreadcrumb.Slug, subItems, secondaryItems);
    }

    private ContentfulReference CheckCurrentArticle(ContentfulReference item)
    {
        if (!item.Sys.Id.Equals(_entry.Sys.Id))
            return item;

        return new()
        {
            Icon = _entry.Icon,
            Title = _entry.Title,
            SunriseDate = _entry.SunriseDate,
            SunsetDate = _entry.SunsetDate,
            Slug = _entry.Slug,
            Image = _entry.Image,
            Teaser = _entry.Teaser,
            Sys = { ContentType = new() { SystemProperties = new() { Id = "article" } } }
        };
    }
}