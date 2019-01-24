using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Showcase
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Subheading { get; set; }
        public string HeroImageUrl { get; set; }
        public string EventSubheading { get; set; }
        public string EventCategory { get; set; }
        public string EventsCategoryOrTag { get; set; }
        public string NewsSubheading { get; set; }
        public string NewsCategoryTag { get; set; }
        public string NewsCategoryOrTag { get; set; }
        public News NewsArticle { get; set; }
        public IEnumerable<SubItem> TertiaryItems { get; }
        public IEnumerable<SubItem> SubItems { get; }
        public string BodySubheading { get; set; }
        public string Body { get; set; }
        public IEnumerable<SubItem> SecondaryItems { get; set; }
        public IEnumerable<SubItem> PrimaryItems { get; set; }
        public string FeaturedItemsSubheading { get; set; }
        public IEnumerable<SubItem> FeaturedItems { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; set; }
        public IEnumerable<Consultation> Consultations { get; set; }
        public IEnumerable<SocialMediaLink> SocialMediaLinks { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public string EmailAlertsTopicId { get; set; }
        public string EmailAlertsText { get; set; }
        public IEnumerable<Alert> Alerts { get; }
        public string KeyFactSubheading { get; }
        public IEnumerable<KeyFact> KeyFacts { get; }
        public Profile Profile { get; }
        public List<Profile> Profiles { get; }
        public string ProfileHeading { get; }
        public string ProfileLink { get; }
        public CallToActionBanner CallToActionBanner { get; }
        public FieldOrder FieldOrder { get; }
        public string Icon { get; }
        public string TriviaSubheading { get; set; }
        public List<InformationList> TriviaSection { get; set; }
        public Video Video { get; set; }

        public Showcase(string slug, 
            string title, 
            IEnumerable<SubItem> secondaryItems, 
            string heroImage, 
            string subheading, 
            string teaser, 
            IEnumerable<Crumb> breadcrumbs,
            IEnumerable<Consultation> consultations, 
            IEnumerable<SocialMediaLink> socialMediaLinks, 
            string eventSubheading, 
            string eventCategory, 
            string newsSubheading, 
            string newsCategoryTag, 
            string bodySubheading, 
            string body, 
            string emailAlertsTopicId, 
            string emailAlertsText, 
            IEnumerable<Alert> alerts, 
            IEnumerable<SubItem> primaryItems,
            string featuredItemsSubheading,
            IEnumerable<SubItem> featuredItems,
            IEnumerable<KeyFact> keyFacts, 
            Profile profile, 
            List<Profile> profiles, 
            FieldOrder fieldOrder, 
            string keyFactSubheading, 
            string icon, 
            IEnumerable<SubItem> subItems,
            IEnumerable<SubItem> tertiaryItems,
            string triviaSubheading,
            List<InformationList> triviaSection, 
            CallToActionBanner callToActionBanner,
            string profileHeading,
            string profileLink,
            Video video)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Subheading = subheading;
            HeroImageUrl = heroImage;
            SecondaryItems = secondaryItems;
            Breadcrumbs = breadcrumbs;
            Consultations = consultations;
            SocialMediaLinks = socialMediaLinks;
            EventSubheading = eventSubheading;
            EventCategory = eventCategory;
            NewsSubheading = newsSubheading;
            NewsCategoryTag = newsCategoryTag;
            BodySubheading = bodySubheading;
            Body = body;
            EmailAlertsTopicId = emailAlertsTopicId;
            EmailAlertsText = emailAlertsText;
            Alerts = alerts;
            KeyFacts = keyFacts;
            PrimaryItems = primaryItems;
            FeaturedItemsSubheading = featuredItemsSubheading;
            FeaturedItems = featuredItems;
            Profile = profile;
            Profiles = profiles;
            FieldOrder = fieldOrder;
            KeyFactSubheading = keyFactSubheading;
            Icon = icon;
            SubItems = subItems;
            TertiaryItems = tertiaryItems;
            TriviaSubheading = triviaSubheading;
            TriviaSection = triviaSection;
            CallToActionBanner = callToActionBanner;
            Video = video;
            ProfileHeading = profileHeading;
            ProfileLink = profileLink;
        }
    }
}