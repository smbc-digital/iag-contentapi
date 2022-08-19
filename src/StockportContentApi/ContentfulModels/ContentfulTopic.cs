using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulTopic : ContentfulReference
    {
        public string Summary { get; set; } = string.Empty;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
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

        public ContentfulCarouselContent CampaignBanner { get; set; } = new ContentfulCarouselContent();
    }
}
