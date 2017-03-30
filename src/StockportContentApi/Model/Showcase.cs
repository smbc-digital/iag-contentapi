using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Showcase
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Subheading { get; set; }
        public string HeroImageUrl { get; set; }
        public IEnumerable<SubItem> FeaturedItems { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; set; }
        
        public Showcase(string slug, string title, IEnumerable<SubItem> featuredItems, string heroImage, string subheading, string teaser, IEnumerable<Crumb> breadcrumbs )
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Subheading = subheading;
            HeroImageUrl = heroImage;
            FeaturedItems = featuredItems;
            Breadcrumbs = breadcrumbs;
        }
    }
}