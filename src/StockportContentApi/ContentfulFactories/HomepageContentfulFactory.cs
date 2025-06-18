namespace StockportContentApi.ContentfulFactories;

public class HomepageContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
                                    IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                    IContentfulFactory<ContentfulCarouselContent, CarouselContent> carouselFactory,
                                    ITimeProvider timeProvider,
                                    IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
                                    IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner> spotlightOnBanner) : IContentfulFactory<ContentfulHomepage, Homepage>
{
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory = subitemFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulCarouselContent, CarouselContent> _carouselFactory = carouselFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = callToActionFactory;
    private readonly IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner> _spotlightOnBanner = spotlightOnBanner;

    public Homepage ToModel(ContentfulHomepage entry)
    {
        string featuredTasksHeading = !string.IsNullOrEmpty(entry.FeaturedTasksHeading)
            ? entry.FeaturedTasksHeading
            : string.Empty;
        
        string featuredTasksSummary = !string.IsNullOrEmpty(entry.FeaturedTasksSummary) 
            ? entry.FeaturedTasksSummary
            : string.Empty;
        
        string backgroundImage = !string.IsNullOrEmpty(entry.BackgroundImage?.File?.Url)
            ? entry.BackgroundImage.File.Url
            : string.Empty;
        
        string foregroundImage = !string.IsNullOrEmpty(entry.ForegroundImage?.File?.Url)
            ? entry.ForegroundImage.File.Url 
            : string.Empty;
        
        string foregroundImageLocation = !string.IsNullOrEmpty(entry.ForegroundImageLocation)
            ? entry.ForegroundImageLocation 
            : string.Empty;
        
        string foregroundImageLink = !string.IsNullOrEmpty(entry.ForegroundImageLink)
            ? entry.ForegroundImageLink 
            : string.Empty;
        
        string foregroundImageAlt = !string.IsNullOrEmpty(entry.ForegroundImage.Description)
            ? entry.ForegroundImage.Description 
            : string.Empty;

        string freeText = !string.IsNullOrEmpty(entry.FreeText) 
            ? entry.FreeText 
            : string.Empty;
        
        string title = !string.IsNullOrEmpty(entry.Title) 
            ? entry.Title 
            : string.Empty;

        List<SubItem> featuredTasks = entry.FeaturedTasks.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(_subitemFactory.ToModel).ToList();

        List<SubItem> featuredTopics = entry.FeaturedTopics.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(_subitemFactory.ToModel).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Select(_alertFactory.ToModel).ToList();

        List<CarouselContent> carouselContents = entry.CarouselContents.Where(subItem => subItem.Sys is not null && ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                                    .Select(_carouselFactory.ToModel).ToList();

        CarouselContent campaignBanner = _carouselFactory.ToModel(entry.CampaignBanner);

        CallToActionBanner callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        CallToActionBanner callToActionPrimary = _callToActionFactory.ToModel(entry.CallToActionPrimary);

        List<SpotlightOnBanner> spotlightOnBanner = entry.SpotlightOnBanner?
                                                            .Where(spotlightOnBanner => ContentfulHelpers.EntryIsNotALink(spotlightOnBanner.Sys))
                                                            .Select(_spotlightOnBanner.ToModel).ToList();

        return new Homepage(featuredTasksHeading,
                            featuredTasksSummary,
                            featuredTasks,
                            featuredTopics,
                            alerts,
                            carouselContents,
                            backgroundImage,
                            foregroundImage,
                            foregroundImageLocation,
                            foregroundImageLink,
                            foregroundImageAlt,
                            freeText,
                            title,
                            entry.EventCategory,
                            entry.MetaDescription,
                            campaignBanner,
                            callToAction,
                            callToActionPrimary,
                            spotlightOnBanner,
                            entry.ImageOverlayText);
    }
}