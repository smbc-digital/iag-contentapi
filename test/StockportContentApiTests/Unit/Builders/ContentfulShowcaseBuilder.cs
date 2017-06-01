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
        private List<ContentfulConsultation> _consultations = new List<ContentfulConsultation>();
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
                FeaturedItems = _featuredItems,
                Breadcrumbs = _breadcrumbs,
                Consultations = _consultations
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
    }
}