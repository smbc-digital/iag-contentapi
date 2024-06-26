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