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
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory;

    public TopicContentfulFactory(
        IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
        IContentfulFactory<ContentfulCarouselContent, CarouselContent> carouselFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
        IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory)
    {
        _subItemFactory = subItemFactory;
        _crumbFactory = crumbFactory;
        _alertFactory = alertFactory;
        _carouselFactory = carouselFactory;
        _dateComparer = new DateComparer(timeProvider);
        _eventBannerFactory = eventBannerFactory;
        _callToActionFactory = callToActionFactory;
        _trustedLogoFactory = trustedLogoFactory;
    }

    public Topic ToModel(ContentfulTopic entry)
    {
        List<SubItem> featuredTasks = entry.FeaturedTasks
                                        .Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(_subItemFactory.ToModel).ToList();

        List<SubItem> subItems = entry.SubItems
                                    .Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys) 
                                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                    .Select(_subItemFactory.ToModel).ToList();

        List<SubItem> secondaryItems = entry.SecondaryItems
                                        .Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(_subItemFactory.ToModel).ToList();

        List<Crumb> breadcrumbs = entry.Breadcrumbs
                                    .Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                    .Select(_crumbFactory.ToModel).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList();

        string backgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
            ? entry.BackgroundImage.File.Url 
            : string.Empty;

        string image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
            ? entry.Image.File.Url 
            : string.Empty;

        EventBanner eventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
            ? _eventBannerFactory.ToModel(entry.EventBanner) 
            : new NullEventBanner();

        bool displayContactUs = entry.DisplayContactUs;
        CarouselContent campaignBanner = _carouselFactory.ToModel(entry.CampaignBanner);
        CallToActionBanner callToAction = _callToActionFactory.ToModel(entry.CallToAction);

        List<TrustedLogo> trustedLogos = entry.TrustedLogos is not null 
            ? entry.TrustedLogos
                .Where(trustedLogo => trustedLogo is not null)
                .Select(_trustedLogoFactory.ToModel).ToList() 
            : new List<TrustedLogo>();

        string logoAreaTitle = entry.LogoAreaTitle;

        IEnumerable<Trivia> trivia = entry.TriviaSection is not null && entry.TriviaSection.Any() 
            ? entry.TriviaSection.Select(trivia => new Trivia(trivia.Title, trivia.Icon, trivia.Body, trivia.Link, trivia.Statistic, trivia.StatisticSubHeading))
            : new List<Trivia>();

        return new Topic(entry.Slug,
                        entry.Name,
                        entry.Teaser,
                        entry.MetaDescription,
                        entry.Summary,
                        entry.Icon,
                        backgroundImage,
                        image,
                        featuredTasks,
                        subItems,
                        secondaryItems,
                        breadcrumbs,
                        alerts,
                        entry.SunriseDate,
                        entry.SunsetDate,
                        eventBanner,
                        campaignBanner,
                        entry.EventCategory,
                        callToAction,
                        trustedLogos,
                        logoAreaTitle,
                        displayContactUs)
        {
            TriviaSection = new TriviaSection(entry.TriviaSubheading, trivia),
            Video = new Video(entry.VideoTitle, entry.VideoTeaser, entry.VideoTag),
            CallToAction = _callToActionFactory.ToModel(entry.CallToAction)
        };
    }
}