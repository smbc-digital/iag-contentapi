using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulTopic : ContentfulReference
    {
        public string Summary { get; set; } = string.Empty;
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public string VideoTitle { get; init; }
        public string VideoTeaser { get; init; }
        public string VideoTag { get; init; }
        public string TriviaSubheading { get; set; } = string.Empty;
        public List<ContentfulTrivia> TriviaSection { get; init; }
        public ContentfulCallToAction CallToAction { get; init; }
        public bool EmailAlerts { get; set; } = false;
        public string EmailAlertsTopicId { get; set; } = string.Empty;
        public string ExpandingLinkTitle { get; set; } = string.Empty;
        public List<ContentfulExpandingLinkBox> ExpandingLinkBoxes { get; set; } = new List<ContentfulExpandingLinkBox>();
        public ContentfulEventBanner EventBanner { get; set; } = new ContentfulEventBanner
        {
            Sys = new SystemProperties { Type = "Entry" }
        };
        public string PrimaryItemTitle { get; set; }
        public bool DisplayContactUs { get; set; } = true;
        public string EventCategory { get; set; }

        public ContentfulCarouselContent CampaignBanner { get; set; } = new ContentfulCarouselContent();
    }
}
