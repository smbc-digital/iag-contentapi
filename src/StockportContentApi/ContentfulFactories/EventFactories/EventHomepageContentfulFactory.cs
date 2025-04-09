namespace StockportContentApi.ContentfulFactories.EventFactories;

public class EventHomepageContentfulFactory(IContentfulFactory<ContentfulCallToActionBanner,
                                            CallToActionBanner> callToActionFactory,
                                            IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                            ITimeProvider timeProvider)
    : IContentfulFactory<ContentfulEventHomepage, EventHomepage>
{
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = callToActionFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);

    public EventHomepage ToModel(ContentfulEventHomepage entry)
    {
        List<string> tags = new()
        {
            entry.TagOrCategory1,
            entry.TagOrCategory2,
            entry.TagOrCategory3,
            entry.TagOrCategory4,
            entry.TagOrCategory5,
            entry.TagOrCategory6,
            entry.TagOrCategory7,
            entry.TagOrCategory8,
            entry.TagOrCategory9,
            entry.TagOrCategory10
        };

        List<EventHomepageRow> rows = new()
        {
            new EventHomepageRow
            {
                IsLatest = true,
                Tag = string.Empty,
                MatchedByTag = true,
                Events = null
            }
        };

        foreach (string tag in tags)
        {
            rows.Add(new EventHomepageRow
            {
                IsLatest = false,
                Tag = tag,
                MatchedByTag = true,
                Events = null
            });
        }

        CallToActionBanner callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        EventHomepage eventHomePage = new(rows)
        {
            MetaDescription = entry.MetaDescription,
            Alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList(),
            GlobalAlerts = entry.GlobalAlerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList(),
            CallToAction = callToAction
        };

        return eventHomePage;
    }
}
