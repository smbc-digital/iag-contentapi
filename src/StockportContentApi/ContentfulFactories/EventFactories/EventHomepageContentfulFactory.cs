namespace StockportContentApi.ContentfulFactories.EventFactories;

public class EventHomepageContentfulFactory : IContentfulFactory<ContentfulEventHomepage, EventHomepage>
{
    private readonly DateComparer _dateComparer;

    public EventHomepageContentfulFactory(ITimeProvider timeProvider)
        => _dateComparer = new DateComparer(timeProvider);

    public EventHomepage ToModel(ContentfulEventHomepage entry)
    {
        List<string> tags = new()
        {
            entry.Tag1,
            entry.Tag2,
            entry.Tag3,
            entry.Tag4,
            entry.Tag5,
            entry.Tag6,
            entry.Tag7,
            entry.Tag8,
            entry.Tag9,
            entry.Tag10
        };

        List<EventHomepageRow> rows = new()
        {
            new EventHomepageRow
            {
                IsLatest = true,
                Tag = string.Empty,
                Events = null
            }
        };

        foreach (string tag in tags)
        {
            rows.Add(new EventHomepageRow
            {
                IsLatest = false,
                Tag = tag,
                Events = null
            });
        }

        EventHomepage eventHomePage = new(rows)
        {
            MetaDescription = entry.MetaDescription,
            Alerts = entry.Alerts
        };

        return eventHomePage;
    }
}
