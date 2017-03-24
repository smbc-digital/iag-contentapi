using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Model
{
    public class Showcase
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Subheading { get; set; }
        public string HeroImage { get; set; }
        public IEnumerable<Topic> FeaturedTopic { get; set; }
        
        public Showcase(string slug, string title, IEnumerable<Topic> featuredTopic, string heroImage, string subheading, string teaser)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Subheading = subheading;
            HeroImage = heroImage;
            FeaturedTopic = featuredTopic;
        }
    }
}