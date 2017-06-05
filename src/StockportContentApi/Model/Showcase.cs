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
        public string NewsSubheading { get; set; }
        public string NewsCategoryTag { get; set; }
        public string NewsCategoryOrTag { get; set; }
        public News NewsArticle { get; set; }
        public IEnumerable<SubItem> FeaturedItems { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; set; }
        public IEnumerable<Consultation> Consultations { get; set; }
        public IEnumerable<SocialMediaLink> SocialMediaLinks { get; set; }
        public IEnumerable<Event> Events { get; set; }

        public Showcase(string slug, string title, IEnumerable<SubItem> featuredItems, string heroImage, string subheading, string teaser, IEnumerable<Crumb> breadcrumbs, IEnumerable<Consultation> consultations, IEnumerable<SocialMediaLink> socialMediaLinks, string eventSubheading, string eventCategory, string newsSubheading, string newsCategoryTag)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Subheading = subheading;
            HeroImageUrl = heroImage;
            FeaturedItems = featuredItems;
            Breadcrumbs = breadcrumbs;
            Consultations = consultations;
            SocialMediaLinks = socialMediaLinks;
            EventSubheading = eventSubheading;
            EventCategory = eventCategory;
            NewsSubheading = newsSubheading;
            NewsCategoryTag = newsCategoryTag;
        }
    }
}