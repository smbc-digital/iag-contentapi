using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class StartPage
    {
        public string Title { get; }
        public string Slug { get; }
        public string Teaser { get; }
        public string Summary { get; }
        public string UpperBody { get; }
        public string FormLinkLabel { get; }
        public string FormLink { get; }
        public string LowerBody { get; }
        public string BackgroundImage { get; }
        public string Icon { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public List<Alert> Alerts { get; }
        public IEnumerable<Alert> AlertsInline { get; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }

        public StartPage(
            string title, string slug, string teaser, string summary, string upperBody, string formLinkLabel,
            string formLink, string lowerBody, string backgroundImage, string icon, IEnumerable<Crumb> breadcrumbs, List<Alert> alerts,  IEnumerable<Alert> inlineAlerts, DateTime sunriseDate, DateTime sunsetDate)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Summary = summary;
            UpperBody = upperBody;
            FormLinkLabel = formLinkLabel;
            FormLink = formLink;
            LowerBody = lowerBody;
            Breadcrumbs = breadcrumbs;
            BackgroundImage = backgroundImage;
            Icon = icon;
            Alerts = alerts;
            AlertsInline = inlineAlerts;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
        }
    }

    public class NullStartPage : StartPage
    {
        public NullStartPage()
            : base(
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, new List<Crumb>(), new List<Alert>(), new List<Alert>(), new DateTime(), new DateTime()) { }
    }
}
