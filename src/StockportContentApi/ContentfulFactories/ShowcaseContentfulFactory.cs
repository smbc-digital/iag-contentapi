namespace StockportContentApi.ContentfulFactories;

public class ShowcaseContentfulFactory : IContentfulFactory<ContentfulShowcase, Showcase>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionBannerContentfulFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
    private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;
    private readonly IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner> _spotlightBannerFactory;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulTrivia, Trivia> _triviaFactory;
    private readonly IContentfulFactory<ContentfulVideo, Video> _videoFactory;

    public ShowcaseContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulProfile, Profile> profileFactory,
        IContentfulFactory<ContentfulTrivia, Trivia> triviaFactory,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionBannerContentfulFactory,
        IContentfulFactory<ContentfulVideo, Video> videoFactory,
        IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner> spotlightBannerFactory)
    {
        _subitemFactory = subitemFactory;
        _crumbFactory = crumbFactory;
        _socialMediaFactory = socialMediaFactory;
        _dateComparer = new(timeProvider);
        _alertFactory = alertFactory;
        _profileFactory = profileFactory;
        _callToActionBannerContentfulFactory = callToActionBannerContentfulFactory;
        _triviaFactory = triviaFactory;
        _videoFactory = videoFactory;
        _spotlightBannerFactory = spotlightBannerFactory;
    }

    public Showcase ToModel(ContentfulShowcase entry)
    {
        string heroImage = entry.HeroImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.HeroImage.SystemProperties)
            ? entry.HeroImage.File.Url
            : string.Empty;

        List<SubItem> primaryItems = entry.PrimaryItems.Where(primItem => ContentfulHelpers.EntryIsNotALink(primItem.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate))
                                        .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<SubItem> secondaryItems = entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<SubItem> featuredItems = entry.FeaturedItems.Where(featItem => ContentfulHelpers.EntryIsNotALink(featItem.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(featItem.SunriseDate, featItem.SunsetDate))
                                        .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<SocialMediaLink> socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                                                    .Select(media => _socialMediaFactory.ToModel(media)).ToList();

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        Profile profile = entry.Profile is not null
            ? _profileFactory.ToModel(entry.Profile)
            : null;

        List<Profile> profiles = entry.Profiles.Where(singleProfile => ContentfulHelpers.EntryIsNotALink(singleProfile.Sys))
                                    .Select(singleProfile => _profileFactory.ToModel(singleProfile)).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert)).ToList();

        List<SubItem> tertiaryItems = entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                        .Select(subItem => _subitemFactory.ToModel(subItem)).ToList();

        List<Trivia> triviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                                        .Select(fact => _triviaFactory.ToModel(fact)).ToList();

        CallToActionBanner callToActionBanner = entry.CallToActionBanner is not null
            ? _callToActionBannerContentfulFactory.ToModel(entry.CallToActionBanner)
            : null;

        Video video = entry.Video is not null
            ? _videoFactory.ToModel(entry.Video)
            : null;

        SpotlightOnBanner spotlightBanner = entry.SpotlightBanner is not null
            ? _spotlightBannerFactory.ToModel(entry.SpotlightBanner)
            : null;

        return new()
        {
            Title = entry.Title,
            Slug = entry.Slug,
            HeroImageUrl = heroImage,
            PrimaryItems = primaryItems,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Subheading = entry.Subheading,
            SecondaryItems = secondaryItems,
            FeaturedItemsSubheading = entry.FeaturedItemsSubheading,
            FeaturedItems = featuredItems,
            SocialMediaLinksSubheading = entry.SocialMediaLinksSubheading,
            SocialMediaLinks = socialMediaLinks,
            EventSubheading = entry.EventSubheading,
            EventCategory = entry.EventCategory,
            EventsReadMoreText = entry.EventsReadMoreText,
            NewsSubheading = entry.NewsSubheading,
            NewsCategoryTag = entry.NewsCategoryTag,
            Breadcrumbs = breadcrumbs,
            BodySubheading = entry.BodySubheading,
            Body = entry.Body,
            Profile = profile,
            ProfileHeading = entry.ProfileHeading,
            ProfileLink = entry.ProfileLink,
            Profiles = profiles,
            FieldOrder = entry.FieldOrder,
            EmailAlertsTopicId = entry.EmailAlertsTopicId,
            EmailAlertsText = entry.EmailAlertsText,
            Alerts = alerts,
            Icon = entry.Icon,
            TertiaryItems = tertiaryItems,
            TriviaSubheading = entry.TriviaSubheading,
            TriviaSection = triviaSection,
            CallToActionBanner = callToActionBanner,
            Video = video,
            TypeformUrl = entry.TypeformUrl,
            SpotlightBanner = spotlightBanner
        };
    }
}