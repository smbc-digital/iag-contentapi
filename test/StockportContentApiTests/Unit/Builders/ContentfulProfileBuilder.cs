using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulProfileBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _icon = "icon";
        private string _subtitle = "subtitle";
        private string _teaser = "teaser";
        private string _quote = "quote";
        private string _body = "body";
        private string _type = "type";
        private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private Asset _backgroundImage = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();

        private List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        { new ContentfulAlertBuilder().Build() }; 
        private List<ContentfulReference> _breadcrumbs = new List<ContentfulReference>
        { new ContentfulReferenceBuilder().Build() };
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        public ContentfulProfile Build()
        {
            return new ContentfulProfile
            {
                Type = _type,
                Title = _title,
                Slug = _slug,
                Subtitle = _subtitle,
                Teaser = _teaser,
                Image = _image,
                Body = _body,
                Icon = _icon,
                BackgroundImage = _backgroundImage,
                Breadcrumbs = _breadcrumbs,
                Sys = _sys
            };
        }

        public ContentfulProfileNew BuildNew()
        {
            return new ContentfulProfileNew
            {
                Title = _title,
                Slug = _slug,
                Subtitle = _subtitle,
                Quote = _quote,
                Image = _image,
                Body = _body,
                Breadcrumbs = _breadcrumbs,
                Sys = _sys,
                Alerts = _alerts
            };
        }

        public ContentfulProfileBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }
    }
}
