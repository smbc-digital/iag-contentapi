using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Model
{
    public class Subhomepage
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public IEnumerable<Topic> FeaturedTopic { get; set; }
        public string HeroImage { get; set; }
        public string Subheading { get; set; }
        public string Teaser { get; set; }

        public Subhomepage(string slug, string title, IEnumerable<Topic> featuredTopic, string heroImage, string subheading, string teaser)
        {
            Slug = slug;
            Title = title;
            FeaturedTopic = featuredTopic;
            HeroImage = heroImage;
            Subheading = subheading;
            Teaser = teaser;
        }
    }
}