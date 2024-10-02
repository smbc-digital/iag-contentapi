namespace StockportContentApi.ContentfulFactories;

public class HomepageContentfulFactory : IContentfulFactory<ContentfulHomepage, Homepage>
{
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulCarouselContent, CarouselContent> _carouselFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
    private readonly IContentfulFactory<IEnumerable<ContentfulSpotlightOnBanner>, IEnumerable<SpotlightOnBanner>> _spotlightOnBanner;

    public HomepageContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, IContentfulFactory<ContentfulGroup, Group> groupFactory, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulCarouselContent, CarouselContent> carouselFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory, IContentfulFactory<IEnumerable<ContentfulSpotlightOnBanner>, IEnumerable<SpotlightOnBanner>> spotlightOnBanner)
    {
        _subitemFactory = subitemFactory;
        _groupFactory = groupFactory;
        _alertFactory = alertFactory;
        _carouselFactory = carouselFactory;
        _dateComparer = new DateComparer(timeProvider);
        _callToActionFactory = callToActionFactory;
        _spotlightOnBanner = spotlightOnBanner;
    }

    public Homepage ToModel(ContentfulHomepage entry)
    {
        string featuredTasksHeading = !string.IsNullOrEmpty(entry.FeaturedTasksHeading) ? entry.FeaturedTasksHeading : string.Empty;
        string featuredTasksSummary = !string.IsNullOrEmpty(entry.FeaturedTasksSummary) ? entry.FeaturedTasksSummary : string.Empty;
        string backgroundImage = !string.IsNullOrEmpty(entry.BackgroundImage?.File?.Url) ? entry.BackgroundImage.File.Url : string.Empty;
        string foregroundImage = !string.IsNullOrEmpty(entry.ForegroundImage?.File?.Url) ? entry.ForegroundImage.File.Url : string.Empty;
        string foregroundImageLocation = !string.IsNullOrEmpty(entry.ForegroundImageLocation) ? entry.ForegroundImageLocation : string.Empty;
        string foregroundImageLink = !string.IsNullOrEmpty(entry.ForegroundImageLink) ? entry.ForegroundImageLink : string.Empty;
        string foregroundImageAlt = !string.IsNullOrEmpty(entry.ForegroundImage.Description) ? entry.ForegroundImage.Description : string.Empty;

        string freeText = !string.IsNullOrEmpty(entry.FreeText) ? entry.FreeText : string.Empty;
        string title = !string.IsNullOrEmpty(entry.Title) ? entry.Title : string.Empty;

        IEnumerable<string> popularSearchTerms = ContentfulHelpers.ConvertToListOfStrings(entry.PopularSearchTerms);

        List<SubItem> featuredTasks =
            entry.FeaturedTasks.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<SubItem> featuredTopics =
            entry.FeaturedTopics.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
                                        _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                        .Select(alert => _alertFactory.ToModel(alert)).ToList();

        List<CarouselContent> carouselContents =
            entry.CarouselContents.Where(subItem => subItem.Sys is not null && ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _carouselFactory.ToModel(item)).ToList();

        CarouselContent campaignBanner = _carouselFactory.ToModel(entry.CampaignBanner);

        Group featuredGroup = 
            entry.FeaturedGroups.Where(group => ContentfulHelpers.EntryIsNotALink(group.Sys)
                && _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo))
                .Select(group => _groupFactory.ToModel(group)).FirstOrDefault();

        CallToActionBanner callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        CallToActionBanner callToActionPrimary = _callToActionFactory.ToModel(entry.CallToActionPrimary);

        IEnumerable<SpotlightOnBanner> spotlightOnBanner = _spotlightOnBanner.ToModel(entry.SpotlightOnBanner);

        return new Homepage(popularSearchTerms, featuredTasksHeading, featuredTasksSummary, featuredTasks,
            featuredTopics, alerts, carouselContents, backgroundImage, foregroundImage, foregroundImageLocation, foregroundImageLink, foregroundImageAlt, freeText, title, featuredGroup, entry.EventCategory, entry.MetaDescription, campaignBanner, callToAction, callToActionPrimary, spotlightOnBanner);
    }
}