using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Builders
{
    public class ContentfulContactUsAreaBuilder
    {
        private string _title { get; set; } = "title";
        private string _slug { get; set; } = "slug";
        private string _teaser { get; set; } = "teaser";
        private string _body { get; set; } = "body";
        private List<ContentfulReference> _primaryItems { get; set; } = new List<ContentfulReference>();

        private List<ContentfulReference> _breadcrumbs = new List<ContentfulReference>
        {
          new ContentfulReferenceBuilder().Build()
        };

        private List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlertBuilder().Build()
        };

        public ContentfulContactUsArea Build()
        {
            return new ContentfulContactUsArea()
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Breadcrumbs = _breadcrumbs,
                Body = _body,
                Alerts = _alerts,
                PrimaryItems = _primaryItems
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

        public ContentfulContactUsAreaBuilder Teaser(string teaser)
        {
            _teaser = teaser;
            return this;
        }

        public ContentfulContactUsAreaBuilder Body(string body)
        {
            _body = body;
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
    }
}