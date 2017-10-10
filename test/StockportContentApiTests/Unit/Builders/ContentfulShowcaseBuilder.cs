using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;
using System;

namespace StockportContentApiTests.Builders
{
    public class ContentfulShowcaseBuilder
    {
        private string _title { get; set; } = "title";
        private string _slug { get; set; } = "slug";
        private string _teaser { get; set; } = "teaser";
        private string _subheading { get; set; } = "subheading";
        private Asset _heroImage { get; set; } = new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private List<ContentfulReference> _featuredItems { get; set; } = new List<ContentfulReference>
        {
            new ContentfulReferenceBuilder().Build()
        };
        private string _eventSubheading { get; set; } = "event-subheading";
        private string _eventCategory { get; set; } = "event-category";
        private string _newsSubheading { get; set; } = "news subheading";
        private string _newsCategoryTag { get; set; } = "news-category-tag";
        private string _bodySubheading { get; set; } = "body subheading";
        private string _body { get; set; } = "body";
        private string _emailAlertsTopicId { get; set; } = "alertId";
        private string _emailAlertsText { get; set; } = "alertText";
        private List<ContentfulConsultation> _consultations = new List<ContentfulConsultation>();
        private List<ContentfulSocialMediaLink> _socialMediaLinks = new List<ContentfulSocialMediaLink>();
        private List<ContentfulReference> _breadcrumbs = new List<ContentfulReference>
        {
          new ContentfulReferenceBuilder().Build()
        };

        private readonly List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlert()
            {
                Title = "Warning alert",
                SubHeading = "This is a warning alert.",
                Body = "This is a warning alert.",
                Severity = "Warning",
                SunriseDate = new DateTime(2016, 6, 30, 23, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(2018, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Sys = new SystemProperties() {Type = "Entry"},
                Slug = "slug"
            },
            new ContentfulAlert()
            {
                Title = "Information alert",
                SubHeading = "test",
                Body = "This is an information alert.",
                Severity = "Information",
                SunriseDate = new DateTime(2016, 6, 30, 23, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(2116, 8, 30, 23, 0, 0, DateTimeKind.Utc),
                Sys = new SystemProperties() {Type = "Entry"},
                Slug = "slug"
            },
            new ContentfulAlert()
            {
                Title = "Error alert",
                SubHeading = string.Empty,
                Body = "This is an error alert.",
                Severity = "Error",
                SunriseDate = new DateTime(2016, 7, 31, 23, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(2116, 8, 30, 23, 0, 0, DateTimeKind.Utc),
                Sys = new SystemProperties() {Type = "Entry"},
                Slug = "slug"
            }
        };

        public ContentfulShowcase Build()
        {
            return new ContentfulShowcase()
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Subheading = _subheading,
                HeroImage = _heroImage,
                EventSubheading = _eventSubheading,
                EventCategory = _eventCategory,
                NewsSubheading = _newsSubheading,
                NewsCategoryTag = _newsCategoryTag,
                FeaturedItems = _featuredItems,
                Breadcrumbs = _breadcrumbs,
                Consultations = _consultations,
                SocialMediaLinks = _socialMediaLinks,
                BodySubheading = _bodySubheading,
                Body = _body,
                EmailAlertsText = _emailAlertsText,
                EmailAlertsTopicId = _emailAlertsTopicId,
                Alerts = _alerts
            };
        }

        public ContentfulShowcaseBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulShowcaseBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulShowcaseBuilder HeroImage(Asset heroImage)
        {
            _heroImage = heroImage;
            return this;
        }

        public ContentfulShowcaseBuilder Teaser(string teaser)
        {
            _teaser = teaser;
            return this;
        }

        public ContentfulShowcaseBuilder Subheading(string subheading)
        {
            _subheading = subheading;
            return this;
        }

        public ContentfulShowcaseBuilder EventSubheading(string subheading)
        {
            _eventSubheading = subheading;
            return this;
        }

        public ContentfulShowcaseBuilder EventCategory(string cat)
        {
            _eventCategory = cat;
            return this;
        }

        public ContentfulShowcaseBuilder NewsSubheading(string newSubheading)
        {
            _newsSubheading = newSubheading;
            return this;
        }

        public ContentfulShowcaseBuilder NewsCategoryTag(string tag)
        {
            _newsCategoryTag = tag;
            return this;
        }

        public ContentfulShowcaseBuilder BodySubheading(string subheading)
        {
            _bodySubheading = subheading;
            return this;
        }

        public ContentfulShowcaseBuilder Body(string body)
        {
            _body = body;
            return this;
        }

        public ContentfulShowcaseBuilder FeaturedItems(List<ContentfulReference> featuredItems)
        {
            _featuredItems = featuredItems;
            return this;
        }

        public ContentfulShowcaseBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
        {
            _breadcrumbs = breadcrumbs;
            return this;
        }

        public ContentfulShowcaseBuilder Consultations(List<ContentfulConsultation> consultations)
        {
            _consultations = consultations;
            return this;
        }

        public ContentfulShowcaseBuilder SocialMediaLinks(List<ContentfulSocialMediaLink> socialMediaLinks)
        {
            _socialMediaLinks = socialMediaLinks;
            return this;
        }
    }
}