namespace StockportContentApi.ContentfulFactories;

public class ShowcaseContentfulFactory : IContentfulFactory<ContentfulShowcase, Showcase>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
    private readonly IContentfulFactory<ContentfulTrivia, Trivia> _triviaFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionBannerContentfulFactory;
    private readonly IContentfulFactory<ContentfulVideo, Video> _videoFactory;
    private readonly IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner> _spotlightBannerFactory;

    public ShowcaseContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulProfile, Profile> profileFactory,
        IContentfulFactory<ContentfulTrivia, Trivia> triviaFactory,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionBannerContentfulFactory,
        IContentfulFactory<ContentfulVideo, Video> videoFactory,
        IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner> spotlightBannerFactory)
    {
        _subitemFactory = subitemFactory;
        _crumbFactory = crumbFactory;
        _socialMediaFactory = socialMediaFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _profileFactory = profileFactory;
        _callToActionBannerContentfulFactory = callToActionBannerContentfulFactory;
        _triviaFactory = triviaFactory;
        _videoFactory = videoFactory;
        _spotlightBannerFactory = spotlightBannerFactory;
    }

    public Showcase ToModel(ContentfulShowcase entry)
    {
        var heroImage = entry.HeroImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.HeroImage.SystemProperties) ?
           entry.HeroImage.File.Url : string.Empty;

        //var socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
        //    .Select(media => _socialMediaFactory.ToModel(media)).ToList();

        // var breadcrumbs =
        //    entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
        //        .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        //var profile = entry.Profile != null
        //    ? _profileFactory.ToModel(entry.Profile)
        //    : null;

        //var profiles = entry.Profiles.Where(singleProfile => ContentfulHelpers.EntryIsNotALink(singleProfile.Sys))
        //    .Select(singleProfile => _profileFactory.ToModel(singleProfile)).ToList();

        //var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
        //                                         _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
        //    .Select(alert => _alertFactory.ToModel(alert)).ToList();

        //var triviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
        //    .Select(fact => _triviaFactory.ToModel(fact)).ToList();

        //var callToActionBanner = entry.CallToActionBanner != null
        //    ? _callToActionBannerContentfulFactory.ToModel(entry.CallToActionBanner)
        //    : null;

        //var video = entry.Video != null
        //    ? _videoFactory.ToModel(entry.Video)
        //    : null;

        //var spotlightBanner = entry.SpotlightBanner != null
        //    ? _spotlightBannerFactory.ToModel(entry.SpotlightBanner)
        //    : null;

        return new Showcase
        {
            Title = entry.Title,
            Slug = entry.Slug,
            HeroImageUrl = heroImage,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Subheading = entry.Subheading,
            FeaturedItemsSubheading = entry.FeaturedItemsSubheading,
            SocialMediaLinksSubheading = entry.SocialMediaLinksSubheading,
            EventSubheading = entry.EventSubheading,
            EventCategory = entry.EventCategory,
            EventsReadMoreText = entry.EventsReadMoreText,
            NewsSubheading = entry.NewsSubheading,
            NewsCategoryTag = entry.NewsCategoryTag,
            //Breadcrumbs = breadcrumbs,
            BodySubheading = entry.BodySubheading,
            Body = entry.Body,
            ProfileHeading = entry.ProfileHeading,
            ProfileLink = entry.ProfileLink,
            EmailAlertsTopicId = entry.EmailAlertsTopicId,
            EmailAlertsText = entry.EmailAlertsText,
            Icon = entry.Icon,
            TriviaSubheading = entry.TriviaSubheading,
            TypeformUrl = entry.TypeformUrl,
            Content = entry.Content,
            SubItems = entry.SubItems
        };
    }
}