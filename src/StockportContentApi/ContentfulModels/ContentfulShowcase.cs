using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulShowcase : ContentfulReference
    {
        public List<ContentfulReference> FeaturedItems { get; set; } = new List<ContentfulReference>();
        public Asset HeroImage { get; set; } = new Asset{File = new File {Url = string.Empty}, SystemProperties = new SystemProperties {Type = "Asset"}};
        public string Subheading { get; set; } = string.Empty;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public string EventSubheading { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public string NewsSubheading { get; set; } = string.Empty;
        public string NewsCategoryTag { get; set; } = string.Empty;
        public string BodySubheading { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulConsultation> Consultations { get; set; } = new List<ContentfulConsultation>();
        public List<ContentfulSocialMediaLink> SocialMediaLinks { get; set; } = new List<ContentfulSocialMediaLink>();
        public string EmailAlertsTopicId { get; set; } = string.Empty;
        public string EmailAlertsText { get; set; } = string.Empty;
        public List<ContentfulReference> PrimaryItems { get; set; } = new List<ContentfulReference>();
    }
}