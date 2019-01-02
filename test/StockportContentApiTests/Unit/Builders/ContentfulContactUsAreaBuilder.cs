using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Builders
{
    public class ContentfulContactUsAreaBuilder
    {
        private string _title { get; set; } = "title";
        private string _slug { get; set; } = "slug";
        private List<ContentfulReference> _primaryItems { get; set; } = new List<ContentfulReference>();

        private List<ContentfulReference> _breadcrumbs = new List<ContentfulReference>
        {
          new ContentfulReferenceBuilder().Build()
        };

        private List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlertBuilder().Build()
        };

        private List<ContentfulInsetText> _insetTexts = new List<ContentfulInsetText>
        {
            new ContentfulInsetTextBuilder().Build()
        };

        private List<ContentfulContactUsCategory> _contactUsCategories = new List<ContentfulContactUsCategory>
        {
            new ContentfulContactUsCategoryBuilder().Build()
        };

        public ContentfulContactUsArea Build()
        {
            return new ContentfulContactUsArea()
            {
                Title = _title,
                Slug = _slug,
                Breadcrumbs = _breadcrumbs,
                Alerts = _alerts,
                InsetText = _insetTexts,
                PrimaryItems = _primaryItems,
                ContactUsCategories = _contactUsCategories
            };
        }

        public ContentfulContactUsAreaBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulContactUsAreaBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulContactUsAreaBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
        {
            _breadcrumbs = breadcrumbs;
            return this;
        }

        public ContentfulContactUsAreaBuilder PrimaryItems(List<ContentfulReference> primaryItems)
        {
            _primaryItems = primaryItems;
            return this;
        }

        public ContentfulContactUsAreaBuilder Alerts(List<ContentfulAlert> alerts)
        {
            _alerts = alerts;
            return this;
        }

        public ContentfulContactUsAreaBuilder InsetTexts(List<ContentfulInsetText> insetTexts)
        {
            _insetTexts = insetTexts;
            return this;
        }

        public ContentfulContactUsAreaBuilder ContentfulContactUsCategories(List<ContentfulContactUsCategory> contactUsCategories)
        {
            _contactUsCategories = contactUsCategories;
            return this;
        }
    }
}