using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Profile
    {
        public string Type { get; }
        public string Title { get; }
        public string Slug { get; }
        public string Subtitle { get; }
        public string Teaser { get; }
        public string Image { get; }
        public string Body { get; }
        public string Icon { get; }
        public string BackgroundImage { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }

        public Profile(string type, string title, string slug, string subtitle, string teaser, string image, string body, string icon, string backgroundImage, IEnumerable<Crumb> breadcrumbs)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Teaser = teaser;
            Image = image;
            Body = body;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Breadcrumbs = breadcrumbs;
        }

        public Profile(string type, string title, string slug, string subtitle, string teaser, string image)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Teaser = teaser;
            Image = image;
        }

        public Profile(string type, string title, string slug, string subtitle, string body, string icon, string backgroundImage, IEnumerable<Crumb> breadcrumbs)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Body = body;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Breadcrumbs = breadcrumbs;
        }
    }
}
