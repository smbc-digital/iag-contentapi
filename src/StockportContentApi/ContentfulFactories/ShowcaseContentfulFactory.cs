using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;
using StockportContentApi.Services.Profile;


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

            var subItems = entry.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                                          && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                 .Select(subItem => _subitemFactory.ToModel(subItem)).ToList();

            var tertiaryItems = entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(subItem => _subitemFactory.ToModel(subItem)).ToList();

            var title = !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            var heroImage = ContentfulHelpers.EntryIsNotALink(entry.HeroImage.SystemProperties)
                ? entry.HeroImage.File.Url
                : string.Empty;

            var teaser = !string.IsNullOrEmpty(entry.Teaser)
                ? entry.Teaser
                : "";

            var subHeading = !string.IsNullOrEmpty(entry.Subheading)
                ? entry.Subheading
                : "";

            var eventSubheading = !string.IsNullOrEmpty(entry.EventSubheading)
                ? entry.EventSubheading
                : "";

            var eventCategory = !string.IsNullOrEmpty(entry.EventCategory)
                ? entry.EventCategory
                : "";

            var newsSubheading = !string.IsNullOrEmpty(entry.NewsSubheading)
                ? entry.NewsSubheading
                : "";

            var newsCategoryTag = !string.IsNullOrEmpty(entry.NewsCategoryTag)
                ? entry.NewsCategoryTag
                : "";

            var bodySubheading = !string.IsNullOrEmpty(entry.BodySubheading)
                ? entry.BodySubheading
                : "";

            var body = !string.IsNullOrEmpty(entry.Body)
                ? entry.Body
                : "";

            var emailAlertsTopicId = !string.IsNullOrEmpty(entry.EmailAlertsTopicId)
                ? entry.EmailAlertsTopicId
                : "";

            var emailAlertsText = !string.IsNullOrEmpty(entry.EmailAlertsText)
                ? entry.EmailAlertsText
                : "";


            var secondaryItems =
                entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

            var primaryItems =
                entry.PrimaryItems.Where(primItem => ContentfulHelpers.EntryIsNotALink(primItem.Sys)
                                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate))
                    .Select(item => _subitemFactory.ToModel(item)).ToList();

            var featuredItemSubheading = !string.IsNullOrEmpty(entry.FeaturedItemsSubheading)
                ? entry.FeaturedItemsSubheading
                : "";

            var featuredItems =
                entry.FeaturedItems.Where(featItem => ContentfulHelpers.EntryIsNotALink(featItem.Sys)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(featItem.SunriseDate, featItem.SunsetDate))
                .Select(item => _subitemFactory.ToModel(item)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var consultations =
                entry.Consultations.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                               .Select(consult => _consultationFactory.ToModel(consult)).ToList();

            var socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                                               .Select(media => _socialMediaFactory.ToModel(media)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
                                           _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                           .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var keyFacts = entry.KeyFacts.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                                               .Select(fact => _keyFactFactory.ToModel(fact)).ToList();

            var profile = entry.Profile == null ? null : _profileFactory.ToModel(entry.Profile);

            var profiles = entry.Profiles.Where(singleProfile => ContentfulHelpers.EntryIsNotALink(singleProfile.Sys))
                .Select(singleProfile => _profileFactory.ToModel(singleProfile)).ToList();
            var callToActionBanner = entry.CallToActionBanner == null ? null : _callToActionBannerContentfulFactory.ToModel(entry.CallToActionBanner);

            var keyFactSubheading = !string.IsNullOrEmpty(entry.KeyFactSubheading)
                ? entry.KeyFactSubheading
                : "";

            var triviaSubheading = !string.IsNullOrEmpty(entry.TriviaSubheading)
                ? entry.TriviaSubheading
                : "";

            var triviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                .Select(fact => _informationListFactory.ToModel(fact)).ToList();

            var video = entry.Video == null ? null : _videoFactory.ToModel(entry.Video);
           
            return new Showcase(
                slug,
                title,
                secondaryItems,
                heroImage,
                subHeading,
                teaser,
                breadcrumbs,
                consultations,
                socialMediaLinks,
                eventSubheading,
                eventCategory,
                newsSubheading,
                newsCategoryTag,
                bodySubheading,
                body,
                emailAlertsTopicId,
                emailAlertsText,
                alerts,
                primaryItems,
                featuredItemSubheading,
                featuredItems,
                keyFacts,
                profile,
                profiles,
                entry.FieldOrder,
                keyFactSubheading,
                entry.Icon,
                subItems,
                tertiaryItems,
                triviaSubheading,
                triviaSection,
                callToActionBanner,
                video
                entry.ProfileHeading,
                entry.ProfileLink
            ).StripData(_httpContextAccessor);
        }
    }
}