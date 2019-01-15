using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Profile
    {
        public string Type { get; }
        public string Title { get; }
        public string Slug { get; }
        public string Subtitle { get; }
        public string LeadParagraph { get; }
        public string Teaser { get; }
        public string Image { get; }
        public string Body { get; }
        public string Icon { get; }
        public string BackgroundImage { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public List<Alert> Alerts { get; }

        public Profile(string type,
            string title,
            string slug,
            string subtitle,
            string leadParagraph,
            string teaser,
            string image,
            string body,
            string icon,
            string backgroundImage,
            IEnumerable<Crumb> breadcrumbs,
            List<Alert> alerts)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            LeadParagraph = leadParagraph;
            Teaser = teaser;
            Image = image;
            Body = body;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
        }

        public Profile(string type,
            string title,
            string slug,
            string subtitle,
            string leadParagraph,
            string teaser,
            string image)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            LeadParagraph = leadParagraph;
            Teaser = teaser;
            Image = image;
        }

        public Profile(string type,
            string title,
            string slug,
            string subtitle,
            string leadParagraph,
            string body,
            string icon,
            string backgroundImage,
            IEnumerable<Crumb> breadcrumbs)
        {
            Type = type;
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            LeadParagraph = leadParagraph;
            Body = body;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Breadcrumbs = breadcrumbs;
        }
    }
}
