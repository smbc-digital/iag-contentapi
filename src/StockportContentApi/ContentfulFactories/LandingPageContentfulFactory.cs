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

    public LandingPage ToModel(ContentfulLandingPage entry)
    {
        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                        .Where(alert => !alert.Severity.Equals("Condolence"))
                        .Select(alert => _alertFactory.ToModel(alert)).ToList();

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        string image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
            entry.Image.File.Url : string.Empty;

        string headerImage = entry.HeaderImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.HeaderImage.SystemProperties) ?
            entry.HeaderImage.File.Url : string.Empty;

        return new LandingPage
        {
            Slug = entry.Slug,
            Title = entry.Title,
            Subtitle = entry.Subtitle,
            Breadcrumbs = breadcrumbs,
            Alerts = alerts,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Image = image,
            HeaderType = entry.HeaderType,
            HeaderImage = headerImage,
            ContentBlocks = entry.ContentBlocks,
            Content = entry.Content
        };
    }
}