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
        public string Quote { get; }
        public string Image { get; }
        public string Body { get; }
        public string Icon { get; }
        public string BackgroundImage { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public List<Alert> Alerts { get; }
        public string Author { get; }
        public string Subject { get; }

        public Profile(string type,
            string title,
            string slug,
            string subtitle,
            string teaser,
            string quote,
            string image,
            string body,
            string icon,
            string backgroundImage,
            IEnumerable<Crumb> breadcrumbs,
            List<Alert> alerts,
            string author,
            string subject)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Teaser = teaser;
            Quote = quote;
            Image = image;
            Body = body;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            Author = author;
            Subject = subject;
        }

        public Profile(string type,
            string title,
            string slug,
            string subtitle,
            string teaser,
            string quote,
            string image,
            string author,
            string subject)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Teaser = teaser;
            Quote = quote;
            Image = image;
            Author = author;
            Subject = subject;
        }

        public Profile(string type,
            string title,
            string slug,
            string subtitle,
            string body,
            string icon,
            string backgroundImage,
            IEnumerable<Crumb> breadcrumbs,
            string author,
            string subject)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Body = body;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Breadcrumbs = breadcrumbs;
            Author = author;
            Subject = subject;
        }
    }
}
