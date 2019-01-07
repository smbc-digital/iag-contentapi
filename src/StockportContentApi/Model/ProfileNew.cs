using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class ProfileNew
    {
        public string Title { get; }
        public string Slug { get; }
        public string LeadParagraph { get; }
        public string Teaser { get; }
        public string Image { get; }
        public string Body { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public List<Alert> Alerts { get; }
        public List<DidYouKnow> DidYouKnowSection { get; }

        public ProfileNew()
        {
            
        }

        public ProfileNew(string title, string slug, string leadParagraph, string teaser, string image, string body, IEnumerable<Crumb> breadcrumbs, List<Alert> alerts, List<DidYouKnow> didYouKnowSection)
        {
            Title = title;
            Slug = slug;
            LeadParagraph = leadParagraph;
            Teaser = teaser;
            Image = image;
            Body = body;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            DidYouKnowSection = didYouKnowSection;
        }

    }
}
