using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Builders
{
    public class ContentfulShowcaseBuilder
    {
        private string _title { get; set; } = "title";
        private string _slug { get; set; } = "slug";
        private string _teaser { get; set; } = "teaser";
        private string _subheading { get; set; } = "subheading";
        private Asset _heroImage { get; set; } = new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private string _eventSubheading { get; set; } = "event-subheading";
        private string _eventCategory { get; set; } = "event-category";
        private string _newsSubheading { get; set; } = "news subheading";
        private string _newsCategoryTag { get; set; } = "news-category-tag";
        private List<ContentfulConsultation> _consultations = new List<ContentfulConsultation>();
        private List<ContentfulSocialMediaLink> _socialMediaLinks = new List<ContentfulSocialMediaLink>();
        private List<Entry<ContentfulSubItem>> _featuredItems { get; set; } = new List<Entry<ContentfulSubItem>>
        {
            new Entry<ContentfulSubItem>
            {
                Fields = new ContentfulSubItemBuilder().Build(),
                SystemProperties = new SystemProperties { Type = "Entry", ContentType = new ContentType() {SystemProperties = new SystemProperties() { Id = "test" } } }
            }
        };

        private List<Entry<ContentfulCrumb>> _breadcrumbs = new List<Entry<ContentfulCrumb>>
        {
            new ContentfulEntryBuilder<ContentfulCrumb>().Fields(new ContentfulCrumbBuilder().Build()).ContentTypeSystemId("showcase").Build()
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
                SocialMediaLinks = _socialMediaLinks
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

        public ContentfulShowcaseBuilder FeaturedItems(List<Entry<ContentfulSubItem>> featuredItems)
        {
            _featuredItems = featuredItems;
            return this;
        }

        public ContentfulShowcaseBuilder Breadcrumbs(List<Entry<ContentfulCrumb>> breadcrumbs)
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