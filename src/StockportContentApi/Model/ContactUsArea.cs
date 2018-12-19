using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class ContactUsArea
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Body { get; set; }
        public IEnumerable<SubItem> PrimaryItems { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; set; }
        public IEnumerable<Alert> Alerts { get; }

        public ContactUsArea(string slug, string title, IEnumerable<Crumb> breadcrumbs, 
            IEnumerable<Alert> alerts, IEnumerable<SubItem> primaryItems) 
        {
            Title = title;
            Slug = slug;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            PrimaryItems = primaryItems;
        }
    }
}