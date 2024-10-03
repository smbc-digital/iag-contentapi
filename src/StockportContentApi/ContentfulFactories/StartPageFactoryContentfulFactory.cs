namespace StockportContentApi.ContentfulFactories;

public class StartPageFactoryContentfulFactory : IContentfulFactory<ContentfulStartPage, StartPage>
{
    private readonly DateComparer _dateComparer;
    private IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;

    public StartPageFactoryContentfulFactory(ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulReference, Crumb> crumbFactory)
    {
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _crumbFactory = crumbFactory;
    }

    public StartPage ToModel(ContentfulStartPage entry)
    {
        List<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert)).ToList();

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        IEnumerable<Alert> alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(alertInline => _alertFactory.ToModel(alertInline));

        string backgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
            ? entry.BackgroundImage.File.Url 
            : string.Empty;

        return new StartPage(entry.Title, entry.Slug, entry.Teaser, entry.Summary, entry.UpperBody,
            entry.FormLinkLabel, entry.FormLink, entry.LowerBody, backgroundImage, entry.Icon,
            breadcrumbs, alerts, alertsInline, entry.SunriseDate, entry.SunsetDate);
    }
}