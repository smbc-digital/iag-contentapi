using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulShowcase
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<ContentfulSubItem> FeaturedItems { get; set; } = new List<ContentfulSubItem>();
        public Asset HeroImage { get; set; } = new Asset{File = new File {Url = string.Empty}, SystemProperties = new SystemProperties {Type = "Asset"}};
        public string Subheading { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public List<ContentfulCrumb> Breadcrumbs { get; set; } = new List<ContentfulCrumb>();
        public string EventSubheading { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public string NewsSubheading { get; set; } = string.Empty;
        public string NewsCategoryTag { get; set; } = string.Empty;
        public string BodySubheading { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<ContentfulConsultation> Consultations { get; set; } = new List<ContentfulConsultation>();
        public List<ContentfulSocialMediaLink> SocialMediaLinks { get; set; } = new List<ContentfulSocialMediaLink>();
    }
}