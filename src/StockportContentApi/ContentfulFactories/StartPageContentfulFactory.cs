namespace StockportContentApi.ContentfulFactories;

public class StartPageContentfulFactory : IContentfulFactory<ContentfulStartPage, StartPage>
{
    private readonly DateComparer _dateComparer;
    private IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;

    public StartPageContentfulFactory(ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulReference, Crumb> crumbFactory)
    {
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _crumbFactory = crumbFactory;
    }

    public StartPage ToModel(ContentfulStartPage entry)
    {
        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList();

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                    .Select(_crumbFactory.ToModel).ToList();

        IEnumerable<Alert> alertsInline = entry.AlertsInline.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(_alertFactory.ToModel);

        string backgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
            ? entry.BackgroundImage.File.Url 
            : string.Empty;

        return new StartPage(entry.Title,
            entry.Slug,
            entry.Teaser,
            entry.Summary,
            entry.UpperBody,
            entry.FormLink,
            entry.LowerBody,
            backgroundImage,
            entry.Icon,
            breadcrumbs,
            alerts,
            alertsInline);
    }
}