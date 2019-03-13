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
        private readonly IContentfulFactory<ContentfulConsultation, Consultation> _consultationFactory;
        private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulKeyFact, KeyFact> _keyFactFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContentfulFactory<ContentfulInformationList, InformationList> _informationListFactory;
        private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionBannerContentfulFactory;
        private readonly IContentfulFactory<ContentfulVideo, Video> _videoFactory;

        public ShowcaseContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, IContentfulFactory<ContentfulReference, Crumb> crumbFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulConsultation, Consultation> consultationFactory, IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulKeyFact, KeyFact> keyFactFactory, IContentfulFactory<ContentfulProfile, Profile> profileFactory,
            IContentfulFactory<ContentfulInformationList, InformationList> informationListFactory,
            IHttpContextAccessor httpContextAccessor,
            IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionBannerContentfulFactory,
            IContentfulFactory<ContentfulVideo, Video> videoFactory)
        {
            _subitemFactory = subitemFactory;
            _crumbFactory = crumbFactory;
            _consultationFactory = consultationFactory;
            _socialMediaFactory = socialMediaFactory;
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
            _keyFactFactory = keyFactFactory;
            _profileFactory = profileFactory;
            _httpContextAccessor = httpContextAccessor;
            _callToActionBannerContentfulFactory = callToActionBannerContentfulFactory;
            _informationListFactory = informationListFactory;
            _videoFactory = videoFactory;
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

            var consultations =
                entry.Consultations.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                    .Select(consult => _consultationFactory.ToModel(consult)).ToList();

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

            var keyFacts = entry.KeyFacts.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                .Select(fact => _keyFactFactory.ToModel(fact)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
                                                     _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var tertiaryItems = entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(subItem => _subitemFactory.ToModel(subItem)).ToList();

            var triviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                .Select(fact => _informationListFactory.ToModel(fact)).ToList();

            var callToActionBanner = entry.CallToActionBanner != null
                ? _callToActionBannerContentfulFactory.ToModel(entry.CallToActionBanner)
                : null;

            var video = entry.Video != null
                ? _videoFactory.ToModel(entry.Video)
                : null;

            return new Showcase
            {
                Title = entry.Title,
                Slug = entry.Slug,
                HeroImageUrl = heroImage,
                PrimaryItems = primaryItems,
                Teaser = entry.Teaser,
                Subheading = entry.Subheading,
                SecondaryItems = secondaryItems,
                FeaturedItemsSubheading = entry.FeaturedItemsSubheading,
                FeaturedItems = featuredItems,
                Consultations = consultations,
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
                KeyFactSubheading = entry.KeyFactSubheading,
                KeyFacts = keyFacts,
                Alerts = alerts,
                Icon = entry.Icon,
                TertiaryItems = tertiaryItems,
                TriviaSubheading = entry.TriviaSubheading,
                TriviaSection = triviaSection,
                CallToActionBanner = callToActionBanner,
                Video = video,
                TypeformUrl = entry.TypeformUrl
            }.StripData(_httpContextAccessor);
        }
    }
}