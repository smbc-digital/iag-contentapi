namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class TopicContentfulFactory : IContentfulFactory<ContentfulTopic, Topic>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox> _expandingLinkBoxFactory;
    private readonly IContentfulFactory<ContentfulCarouselContent, CarouselContent> _carouselFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
    private readonly IContentfulFactory<ContentfulGroupBranding, GroupBranding> _topicBrandingFactory;
    //02/11/23 hotfix CTA backwards compatibility for HealthyStockport
    private readonly IContentfulFactory<ContentfulCallToAction, CallToAction> _callToActionBannerFactory;

    public TopicContentfulFactory(
        IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
        IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox> expandingLinkBoxFactory,
        IContentfulFactory<ContentfulCarouselContent, CarouselContent> carouselFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
        IContentfulFactory<ContentfulGroupBranding, GroupBranding> topicBrandingFactory,
        IContentfulFactory<ContentfulCallToAction, CallToAction> callToActionBannerFactory)
    {
        _subItemFactory = subItemFactory;
        _crumbFactory = crumbFactory;
        _alertFactory = alertFactory;
        _carouselFactory = carouselFactory;
        _dateComparer = new DateComparer(timeProvider);
        _eventBannerFactory = eventBannerFactory;
        _expandingLinkBoxFactory = expandingLinkBoxFactory;
        _callToActionFactory = callToActionFactory;
        _topicBrandingFactory = topicBrandingFactory;
        _callToActionBannerFactory = callToActionBannerFactory;
    }

    public Topic ToModel(ContentfulTopic entry)
    {
        var subItems = entry.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                     .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        var secondaryItems = entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                     .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        var tertiaryItems = entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                     .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                           .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                 .Where(alert => !alert.Severity.Equals("Condolence"))
                                 .Select(alert => _alertFactory.ToModel(alert)).ToList();

        var backgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                    ? entry.BackgroundImage.File.Url : string.Empty;

        var image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                    ? entry.Image.File.Url : string.Empty;

        var eventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
                            ? _eventBannerFactory.ToModel(entry.EventBanner) : new NullEventBanner();

        var expandingLinkBoxes =
            entry.ExpandingLinkBoxes.Where(e => ContentfulHelpers.EntryIsNotALink(e.Sys))
                .Select(e => _expandingLinkBoxFactory.ToModel(e)).ToList();

        var primaryItemTitle = entry.PrimaryItemTitle;

        var displayContactUs = entry.DisplayContactUs;

        var campaignBanner = _carouselFactory.ToModel(entry.CampaignBanner);

        var callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        var topicBranding = entry.TopicBranding != null ? entry.TopicBranding.Where(o => o != null).Select(branding => _topicBrandingFactory.ToModel(branding)).ToList() : new List<GroupBranding>();

        var logoAreaTitle = entry.LogoAreaTitle;
        var callToActionBanner = _callToActionBannerFactory.ToModel(entry.CallToActionBanner);

        IEnumerable<Trivia> trivia = entry.TriviaSection is not null && entry.TriviaSection.Any() ?
            entry.TriviaSection.Select(trivia => new Trivia(trivia.Name, trivia.Icon, trivia.Text, trivia.Link))
            : new List<Trivia>();

        return new Topic(entry.Slug, entry.Name, entry.Teaser, entry.MetaDescription, entry.Summary, entry.Icon, backgroundImage, image,
            subItems, secondaryItems, tertiaryItems, breadcrumbs, alerts, entry.SunriseDate, entry.SunsetDate,
            entry.EmailAlerts, entry.EmailAlertsTopicId, eventBanner, entry.ExpandingLinkTitle, campaignBanner, entry.EventCategory,
            callToActionBanner, callToAction, topicBranding, logoAreaTitle, expandingLinkBoxes, primaryItemTitle, displayContactUs)
        {
            TriviaSection = new TriviaSection(entry.TriviaSubheading, trivia),
            Video = new Video(entry.VideoTitle, entry.VideoTeaser, entry.VideoTag),
            CallToAction = _callToActionFactory.ToModel(entry.CallToAction),
            CallToActionBanner = _callToActionBannerFactory.ToModel(entry.CallToActionBanner)
        };
    }
}