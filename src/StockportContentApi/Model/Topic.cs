using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Topic
    {
        public string Slug { get; }
        public string Name { get; }
        public string Teaser { get; }
        public string Summary { get;}
        public string Icon { get; }
        public string BackgroundImage { get; }
        public IEnumerable<SubItem> SubItems { get; }
        public IEnumerable<SubItem> SecondaryItems { get; }
        public IEnumerable<SubItem> TertiaryItems { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public IEnumerable<Alert> Alerts { get; private set; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }
        public bool EmailAlerts { get; }
        public string EmailAlertsTopicId { get; }

        public Topic(string slug, string name, string teaser, string summary, string icon, string backgroundImage,
            IEnumerable<SubItem> subItems, IEnumerable<SubItem> secondayItems, IEnumerable<SubItem> tertiaryItems, IEnumerable<Crumb> breadcrumbs, IEnumerable<Alert> alerts, DateTime sunriseDate, DateTime sunsetDate, bool emailAlerts, string emailAlertsTopicId)
        {
            Slug = slug;
            Name = name;
            Teaser = teaser;
            Summary = summary;
            Icon = icon;
            BackgroundImage = backgroundImage;
            SubItems = subItems;
            SecondaryItems = secondayItems;
            TertiaryItems = tertiaryItems;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            EmailAlerts = emailAlerts;
            EmailAlertsTopicId = emailAlertsTopicId;
        }

        internal void SetAlerts(List<Alert> alertsFiltered)
        {
            Alerts = alertsFiltered;
        }
    }

    public class NullTopic : Topic
    {
        public NullTopic() : base(
            string.Empty, 
            string.Empty, 
            string.Empty, 
            string.Empty, 
            string.Empty, 
            string.Empty,
            new List<SubItem>(), 
            new List<SubItem>(), 
            new List<SubItem>(), 
            new List<Crumb>(), 
            new List<Alert>(), 
            DateTime.MinValue, 
            DateTime.MinValue,
            false,
            string.Empty) { }
    }
}
