using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class ContactUsArea
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Body { get; set; }
        public string CategoriesTitle { get; set; }
        public IEnumerable<SubItem> PrimaryItems { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; set; }
        public IEnumerable<Alert> Alerts { get; }
        public IEnumerable<InsetText> InsetTexts { get; }
        public IEnumerable<ContactUsCategory> ContactUsCategories { get; set; }

        public string MetaDescription { get; set; }

        public ContactUsArea(string slug, string title, string categoriesTitle, IEnumerable<Crumb> breadcrumbs, 
            IEnumerable<Alert> alerts, IEnumerable<InsetText> insetTexts, IEnumerable<SubItem> primaryItems, IEnumerable<ContactUsCategory> contactUsCategories, string metaDescription) 
        {
            Title = title;
            Slug = slug;
            CategoriesTitle = categoriesTitle;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            InsetTexts = insetTexts;
            PrimaryItems = primaryItems;
            ContactUsCategories = contactUsCategories;
            MetaDescription = metaDescription;
        }
    }
}