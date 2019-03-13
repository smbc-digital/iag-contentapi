using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulShowcase : ContentfulReference
    {
        public Asset HeroImage { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<ContentfulReference> PrimaryItems { get; set; } = new List<ContentfulReference>();
        public string Subheading { get; set; } = string.Empty;
        public string FeaturedItemsSubheading { get; set; } = string.Empty;
        public List<ContentfulReference> FeaturedItems { get; set; } = new List<ContentfulReference>();
        public List<ContentfulConsultation> Consultations { get; set; } = new List<ContentfulConsultation>();
        public string SocialMediaLinksSubheading { get; set; } = string.Empty;
        public List<ContentfulSocialMediaLink> SocialMediaLinks { get; set; } = new List<ContentfulSocialMediaLink>();
        public string EventSubheading { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public string EventsReadMoreText { get; set; } = string.Empty;
        public string NewsSubheading { get; set; } = string.Empty;
        public string NewsCategoryTag { get; set; } = string.Empty;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public string BodySubheading { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public ContentfulProfile Profile { get; set; } = null;
        public string ProfileHeading { get; set; } = string.Empty;
        public string ProfileLink { get; set; } = string.Empty;
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public FieldOrder FieldOrder { get; set; } = new FieldOrder();
        public string EmailAlertsTopicId { get; set; } = string.Empty;
        public string EmailAlertsText { get; set; } = string.Empty;
        public string KeyFactSubheading { get; set; } = string.Empty;
        public List<ContentfulKeyFact> KeyFacts { get; set; } = new List<ContentfulKeyFact>();
        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public string TriviaSubheading { get; set; } = string.Empty;
        public List<ContentfulInformationList> TriviaSection { get; set; } = new List<ContentfulInformationList>();
        public ContentfulCallToActionBanner CallToActionBanner { get; set; } = null;
        public ContentfulVideo Video { get; set; }
        public string TypeformUrl { get; set; } = string.Empty;
    }
}