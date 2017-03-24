using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Builders
{
    public class ContentfulShowcaseBuilder
    {
        private string _title { get; set; } = "title";
        private string _slug { get; set; } = "slug";
        private string _teaser { get; set; } = "teaser";
        private string _subheading { get; set; } = "subheading";
        private Asset _heroImage { get; set; } = new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
//        private List<Entry<ContentfulTopic>> _featuredTopic { get; set; } = new List<Entry<ContentfulTopic>> {new Entry<ContentfulTopic> {Fields = new ContentfulTopic() {Name = "Topic Test", Teaser = "Teaser", Slug = "Slug", Summary = "Summary"}, SystemProperties = new SystemProperties { Type = "Entry"  } } };
        private List<Entry<ContentfulTopic>> _featuredTopic { get; set; } = new List<Entry<ContentfulTopic>>();

        public ContentfulShowcase Build()
        {
            return new ContentfulShowcase()
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Subheading = _subheading,
                HeroImage = _heroImage,
                FeaturedTopic = _featuredTopic,
            };
        }

        public ContentfulShowcaseBuilder Slug(string slug)
        {
            _slug = slug; 
            return this;
        }
    }
}