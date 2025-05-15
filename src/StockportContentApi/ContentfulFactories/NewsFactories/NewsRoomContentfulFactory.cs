namespace StockportContentApi.ContentfulFactories.NewsFactories;

[ExcludeFromCodeCoverage]
public class NewsRoomContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                    ITimeProvider timeProvider,
                                    IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory) : IContentfulFactory<ContentfulNewsRoom, Newsroom>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = callToActionFactory;

    public Newsroom ToModel(ContentfulNewsRoom entry)
    {
        IEnumerable<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                        .Select(_alertFactory.ToModel);

        CallToActionBanner callToAction = entry.CallToAction is null 
            ? null 
            : _callToActionFactory.ToModel(entry.CallToAction);

        return new Newsroom(alerts.ToList(), entry.EmailAlerts, entry.EmailAlertsTopicId, callToAction);
    }
}