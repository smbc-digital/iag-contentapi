using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;


namespace StockportContentApi.ContentfulFactories
{
    public class ShowcaseContentfulFactory : IContentfulFactory<ContentfulShowcase, Showcase>
    {
        private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
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
            IHttpContextAccessor httpContextAccessor,
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
            _httpContextAccessor = httpContextAccessor;
            _callToActionBannerContentfulFactory = callToActionBannerContentfulFactory;
            _triviaFactory = triviaFactory;
            _videoFactory = videoFactory;
            _spotlightBannerFactory = spotlightBannerFactory;
        }

        public Showcase ToModel(ContentfulShowcase entry)
        {
            var heroImage = ContentfulHelpers.EntryIsNotALink(entry.HeroImage.SystemProperties)
                ? entry.HeroImage.File.Url
                : string.Empty;

            var primaryItems =
                entry.PrimaryItems.Where(primItem => ContentfulHelpers.EntryIsNotALink(primItem.Sys)
                                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate))
                    .Select(item => _subitemFactory.ToModel(item)).ToList();

            var secondaryItems =
                entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                                      && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                    .Select(item => _subitemFactory.ToModel(item)).ToList();

            var featuredItems =
                entry.FeaturedItems.Where(featItem => ContentfulHelpers.EntryIsNotALink(featItem.Sys)
                                                      && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(featItem.SunriseDate, featItem.SunsetDate))
                    .Select(item => _subitemFactory.ToModel(item)).ToList();

            var socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                .Select(media => _socialMediaFactory.ToModel(media)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var profile = entry.Profile != null
                ? _profileFactory.ToModel(entry.Profile)
                : null;

            var profiles = entry.Profiles.Where(singleProfile => ContentfulHelpers.EntryIsNotALink(singleProfile.Sys))
                .Select(singleProfile => _profileFactory.ToModel(singleProfile)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
                                                     _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var tertiaryItems = entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(subItem => _subitemFactory.ToModel(subItem)).ToList();

            var triviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                .Select(fact => _triviaFactory.ToModel(fact)).ToList();

            var callToActionBanner = entry.CallToActionBanner != null
                ? _callToActionBannerContentfulFactory.ToModel(entry.CallToActionBanner)
                : null;

            var video = entry.Video != null
                ? _videoFactory.ToModel(entry.Video)
                : null;

            var spotlightBanner = entry.SpotlightBanner != null
                ? _spotlightBannerFactory.ToModel(entry.SpotlightBanner)
                : null;

            return new Showcase
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
            }.StripData(_httpContextAccessor);
        }
    }
}