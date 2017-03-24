using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Builders
{
    public class ContentfulSubhomepageBuilder
    {
        private string _slug { get; set; } = "slug";
        private string _title { get; set; } = "title";
        private List<Entry<ContentfulTopic>> _featuredTopic { get; set; } = new List<Entry<ContentfulTopic>>();
        private Asset _heroImage { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private string _subheading { get; set; } = "subheading";
        private string _teaser { get; set; } = "teaser";

        public ContentfulSubhomepage Build()
        {
            return new ContentfulSubhomepage()
            {
                Slug = _slug,
                Title = _title,
                FeaturedTopic = _featuredTopic,
                HeroImage = _heroImage,
                Subheading = _subheading,
                Teaser = _teaser
            };
        }

        public ContentfulSubhomepageBuilder Slug(string slug)
        {
            _slug = slug; 
            return this;
        }
    }
}