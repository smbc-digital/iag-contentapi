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
        var featuredTasksHeading = !string.IsNullOrEmpty(entry.FeaturedTasksHeading) ? entry.FeaturedTasksHeading : string.Empty;
        var featuredTasksSummary = !string.IsNullOrEmpty(entry.FeaturedTasksSummary) ? entry.FeaturedTasksSummary : string.Empty;
        var backgroundImage = !string.IsNullOrEmpty(entry.BackgroundImage?.File?.Url) ? entry.BackgroundImage.File.Url : string.Empty;
        var foregroundImage = !string.IsNullOrEmpty(entry.ForegroundImage?.File?.Url) ? entry.ForegroundImage.File.Url : string.Empty;
        var foregroundImageLocation = !string.IsNullOrEmpty(entry.ForegroundImageLocation) ? entry.ForegroundImageLocation : string.Empty;
        var foregroundImageLink = !string.IsNullOrEmpty(entry.ForegroundImageLink) ? entry.ForegroundImageLink : string.Empty;
        var foregroundImageAlt = !string.IsNullOrEmpty(entry.ForegroundImage.Description) ? entry.ForegroundImage.Description : string.Empty;

        var freeText = !string.IsNullOrEmpty(entry.FreeText) ? entry.FreeText : string.Empty;

        var popularSearchTerms = ContentfulHelpers.ConvertToListOfStrings(entry.PopularSearchTerms);

        var featuredTasks =
            entry.FeaturedTasks.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

        var featuredTopics =
            entry.FeaturedTopics.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

        var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
                                        _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                        .Select(alert => _alertFactory.ToModel(alert)).ToList();

        var carouselContents =
            entry.CarouselContents.Where(subItem => subItem.Sys is not null && ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _carouselFactory.ToModel(item)).ToList();

        var campaignBanner = _carouselFactory.ToModel(entry.CampaignBanner);

        var featuredGroup = 
            entry.FeaturedGroups.Where(group => ContentfulHelpers.EntryIsNotALink(group.Sys)
                && _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo))
                .Select(group => _groupFactory.ToModel(group)).FirstOrDefault();

        var callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        var callToActionPrimary = _callToActionFactory.ToModel(entry.CallToActionPrimary);

        var spotlightOnBanner = _spotlightOnBanner.ToModel(entry.SpotlightOnBanner);

        return new Homepage(popularSearchTerms, featuredTasksHeading, featuredTasksSummary, featuredTasks,
            featuredTopics, alerts, carouselContents, backgroundImage, foregroundImage, foregroundImageLocation, foregroundImageLink, foregroundImageAlt, freeText, featuredGroup, entry.EventCategory, entry.MetaDescription, campaignBanner, callToAction, callToActionPrimary, spotlightOnBanner);
    }
}