namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class ParentTopicContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
                                        ITimeProvider timeProvider) : IContentfulFactory<ContentfulArticle, Topic>
{
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory = subItemFactory;
    private ContentfulArticle _entry;

    public Topic ToModel(ContentfulArticle entry)
    {
        _entry = entry;

        ContentfulReference topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(o => o.Sys.ContentType.SystemProperties.Id.Equals("topic"));

        if (topicInBreadcrumb is null)
            return new NullTopic();

        List<SubItem> subItems = topicInBreadcrumb.SubItems.Select(CheckCurrentArticle)
                                    .Where(subItem => subItem is not null 
                                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                    .Select(_subItemFactory.ToModel).ToList();

        List<SubItem> secondaryItems = topicInBreadcrumb.SecondaryItems.Select(CheckCurrentArticle)
                                        .Where(subItem => subItem is not null
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(_subItemFactory.ToModel).ToList();

        return new(topicInBreadcrumb.Title, topicInBreadcrumb.Slug, subItems, secondaryItems);
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