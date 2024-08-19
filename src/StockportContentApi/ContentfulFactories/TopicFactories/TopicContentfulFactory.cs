namespace StockportContentApi.ContentfulFactories.TopicFactories;

public class TopicContentfulFactory : IContentfulFactory<ContentfulTopic, Topic>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulCarouselContent, CarouselContent> _carouselFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
    private readonly IContentfulFactory<ContentfulGroupBranding, GroupBranding> _topicBrandingFactory;

    public TopicContentfulFactory(
        IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
        IContentfulFactory<ContentfulCarouselContent, CarouselContent> carouselFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
        IContentfulFactory<ContentfulGroupBranding, GroupBranding> topicBrandingFactory)
    {
        _subItemFactory = subItemFactory;
        _crumbFactory = crumbFactory;
        _alertFactory = alertFactory;
        _carouselFactory = carouselFactory;
        _dateComparer = new DateComparer(timeProvider);
        _eventBannerFactory = eventBannerFactory;
        _callToActionFactory = callToActionFactory;
        _topicBrandingFactory = topicBrandingFactory;
    }

    public Topic ToModel(ContentfulTopic entry)
    {
        List<SubItem> featuredTasks =
            entry.FeaturedTasks.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _subItemFactory.ToModel(item)).ToList();

        List<SubItem> subItems = entry.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                     .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        List<SubItem> secondaryItems = entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                     .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                           .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                 .Where(alert => !alert.Severity.Equals("Condolence"))
                                 .Select(alert => _alertFactory.ToModel(alert)).ToList();

        string backgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                    ? entry.BackgroundImage.File.Url : string.Empty;

        string image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                    ? entry.Image.File.Url : string.Empty;

        EventBanner eventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
                            ? _eventBannerFactory.ToModel(entry.EventBanner) : new NullEventBanner();

        bool displayContactUs = entry.DisplayContactUs;

        CarouselContent campaignBanner = _carouselFactory.ToModel(entry.CampaignBanner);

        CallToActionBanner callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        List<GroupBranding> topicBranding = entry.TopicBranding is not null ? entry.TopicBranding.Where(_ => _ is not null).Select(branding => _topicBrandingFactory.ToModel(branding)).ToList() : new List<GroupBranding>();

        string logoAreaTitle = entry.LogoAreaTitle;

        IEnumerable<Trivia> trivia = entry.TriviaSection is not null && entry.TriviaSection.Any() ?
            entry.TriviaSection.Select(trivia => new Trivia(trivia.Name, trivia.Icon, trivia.Body, trivia.Link))
            : new List<Trivia>();

        return new Topic(entry.Slug, entry.Name, entry.Teaser, entry.MetaDescription, entry.Summary, entry.Icon, backgroundImage, image, featuredTasks, subItems, secondaryItems, breadcrumbs, alerts, entry.SunriseDate, entry.SunsetDate, entry.EmailAlerts, entry.EmailAlertsTopicId, eventBanner, campaignBanner, 
        entry.EventCategory, callToAction, topicBranding, logoAreaTitle, displayContactUs)
        {
            TriviaSection = new TriviaSection(entry.TriviaSubheading, trivia),
            Video = new Video(entry.VideoTitle, entry.VideoTeaser, entry.VideoTag),
            CallToAction = _callToActionFactory.ToModel(entry.CallToAction),
        };
    }
}