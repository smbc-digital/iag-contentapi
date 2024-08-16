namespace StockportContentApi.ContentfulFactories;

public class LandingPageContentfulFactory : IContentfulFactory<ContentfulLandingPage, LandingPage>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;

    public LandingPageContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory)
    {
        _crumbFactory = crumbFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
    }

    public LandingPage ToModel(ContentfulLandingPage entry) => entry is null
        ? null
        : new()
        {
            Slug = entry.Slug,
            Title = entry.Title,
            Subtitle = entry.Subtitle,
            Breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                        .Select(crumb => _crumbFactory.ToModel(crumb)).ToList(),
            Alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert)).ToList(),
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
                            entry.Image.File.Url : string.Empty,
            HeaderType = entry.HeaderType,
            HeaderImage = entry.HeaderImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.HeaderImage.SystemProperties) ?
                                entry.HeaderImage.File.Url : string.Empty,
            HeaderTheme = entry.HeaderTheme,
            ContentBlocks = entry.ContentBlocks,
            Content = entry.Content
        };
}