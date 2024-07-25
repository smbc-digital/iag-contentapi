namespace StockportContentApi.ContentfulFactories.NewsFactories;

public class NewsRoomContentfulFactory : IContentfulFactory<ContentfulNewsRoom, Newsroom>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly DateComparer _dateComparer;

    public NewsRoomContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory, ITimeProvider timeProvider)
    {
        _alertFactory = alertFactory;
        _dateComparer = new DateComparer(timeProvider);
    }

    public Newsroom ToModel(ContentfulNewsRoom entry)
    {
        var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert));

        return new Newsroom(alerts.ToList(), entry.EmailAlerts, entry.EmailAlertsTopicId);
    }
}