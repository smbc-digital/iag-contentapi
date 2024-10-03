namespace StockportContentApi.ContentfulFactories;

public class LandingPageContentfulFactory : IContentfulFactory<ContentfulLandingPage, LandingPage>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulReference, ContentBlock> _contentBlockFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly DateComparer _dateComparer;

    public LandingPageContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulReference, ContentBlock> contentBlockFactory)
    {
        _crumbFactory = crumbFactory;
        _dateComparer = new(timeProvider);
        _alertFactory = alertFactory;
        _contentBlockFactory = contentBlockFactory;
    }

    public LandingPage ToModel(ContentfulLandingPage entry)
    {
        if (entry is null)
            return null;

        MediaAsset image = new();

        if (entry.Image is not null && entry.Image.File is not null)
            image = new()
            {
                Url = entry.Image.File.Url,
                Description = entry.Image.Description
            };

        MediaAsset headerImage = new();

        if (entry.HeaderImage is not null && entry.HeaderImage.File is not null)
            headerImage = new()
            {
                Url = entry.HeaderImage.File.Url,
                Description = entry.HeaderImage.Description
            };

        return new()
        {
            Slug = entry.Slug,
            Title = entry.Title,
            Subtitle = entry.Subtitle,
            Breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                            .Select(crumb => _crumbFactory.ToModel(crumb)).ToList(),
            
            Alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                        .Where(alert => !alert.Severity.Equals("Condolence"))
                        .Select(alert => _alertFactory.ToModel(alert)).ToList(),

            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Icon = entry.Icon,
            Image = image,
            HeaderType = entry.HeaderType,
            HeaderImage = headerImage,
            HeaderColourScheme = entry.HeaderColourScheme,
            PageSections = entry.PageSections.Where(contentBlock => ContentfulHelpers.EntryIsNotALink(contentBlock.Sys))
                            .Select(contentBlock => _contentBlockFactory.ToModel(contentBlock)).ToList()
        };
    }
}