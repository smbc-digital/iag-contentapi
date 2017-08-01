using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;
using System;

namespace StockportContentApiTests.Builders
{
    public class ContentfulStartPageBuilder
    {
        private string _title { get; set; } = "Start Page";
        private string _slug { get; set; } = "startPageSlug";
        private string _teaser { get; set; } = "this is a teaser";
        private string _summary { get; set; } = "This is a summary";
        private string _upperBody { get; set; } = "An upper body";
        private string _formLinkLabel { get; set; } = "Start now";
        private string _formLink { get; set; } = "http://start.com";
        private string _lowerBody { get; set; } = "Lower body";
        private string _backgroundImage { get; set; } = "image.jpg";
        private string _icon { get; set; } = "icon";
       
        private List<ContentfulReference> _breadcrumbs = new List<ContentfulReference>
        {
          new ContentfulReferenceBuilder().Build()
        };

        private List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                Sys = new SystemProperties() {Type = "Entry"}
            }
        };

       public ContentfulStartPage Build()
        {
            return new ContentfulStartPage()
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Summary = _summary,
                UpperBody = _upperBody,
                FormLinkLabel = _formLinkLabel,
                FormLink = _formLink,
                LowerBody = _lowerBody,
                BackgroundImage = _backgroundImage,
                Icon = _icon,
                Breadcrumbs = _breadcrumbs,
                Alerts = _alerts
            };
        }

        public ContentfulStartPageBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulStartPageBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulStartPageBuilder Teaser(string teaser)
        {
            _teaser = teaser;
            return this;
        }
    }
}