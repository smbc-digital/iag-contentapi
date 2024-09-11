namespace StockportContentApi.ContentfulFactories;

public class LandingPageContentfulFactory : IContentfulFactory<ContentfulLandingPage, LandingPage>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;

    public LandingPageContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulReference, SubItem> subItemFactory)
    {
        _crumbFactory = crumbFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _subItemFactory = subItemFactory;
    }

    public LandingPage ToModel(ContentfulLandingPage entry)
    {
        if(entry is null)
            return null;

        MediaAsset image = new();
        if (entry.Image is not null && entry.Image.File is not null)
        {
            image = new MediaAsset
            {
                Url = entry.Image.File.Url,
                Description = entry.Image.Description
            };
        }

        MediaAsset headerImage = new();
        if (entry.HeaderImage is not null && entry.HeaderImage.File is not null)
        {
            headerImage = new MediaAsset
            {
                Url = entry.HeaderImage.File.Url,
                Description = entry.HeaderImage.Description
            };
        }
        
        return new LandingPage(){
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
            Icon = entry.Icon,
            Image = image,
            HeaderType = entry.HeaderType,
            HeaderImage = headerImage,
            HeaderColourScheme = entry.HeaderColourScheme,
            PageSections = entry.PageSections.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                    .Select(subItem => _subItemFactory.ToModel(subItem)).ToList()
        };
    }
}