using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class SubItem
    {
        public readonly string Slug;
        public readonly string Title;
        public readonly string Teaser;
        public readonly string Icon;
        public readonly string Type;        
        public readonly DateTime SunriseDate;
        public readonly DateTime SunsetDate;
        public readonly string Image;
        public readonly List<SubItem> SubItems;


        public SubItem(string slug, string title, string teaser, string icon, string type, DateTime sunriseDate, DateTime sunsetDate, string image, List<SubItem> subItems)
        {
            Slug = slug;
            Teaser = teaser;
            Title = title;
            Icon = icon;
            Type = type;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            Image = image;
            SubItems = subItems;
        }
    }
}
