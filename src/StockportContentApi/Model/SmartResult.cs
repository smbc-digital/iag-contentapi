using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class SmartResult
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Subheading { get; set; }
        public string Icon { get; set; }
        public string Body { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; } 

        public SmartResult(string title, string slug, string subheading, string icon, string body, string buttonText, string buttonLink)
        {
            Title = title;
            Slug = slug;
            Subheading = subheading;
            Icon = icon;
            Body = body;
            ButtonText = buttonText;
            ButtonLink = buttonLink;
        }
    }
}
