using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulNewsBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _teaser = "teaser";
        private string _purpose = "purpose";
        private string _imageUrl = "image.jpg";
        private string _body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        private DateTime _sunriseDate = new DateTime(2016, 06, 30, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(2017, 01, 30, 23, 0, 0, DateTimeKind.Utc);
        private List<string> _tags = new List<string> { "Bramall Hall", "Events" };
        private List<ContentfulAlert> _alerts = new List<ContentfulAlert> { new ContentfulAlertBuilder().Build() };
        private List<Asset> _documents = new List<Asset>();

        private List<string> _categories = new List<string> { "A category" };

        public ContentfulNews Build()
        {
            return new ContentfulNews
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Purpose = _purpose,
                Image = new ContentfulAssetBuilder().Url(_imageUrl).Build(),
                Body = _body,
                SunriseDate  = _sunriseDate,
                SunsetDate  = _sunsetDate,
                Tags = _tags,
                Alerts  = _alerts,
                Documents  = _documents,
                Categories  = _categories
            };
        }

        public ContentfulNewsBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulNewsBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulNewsBuilder Teaser(string teaser)
        {
            _teaser = teaser;
            return this;
        }

        public ContentfulNewsBuilder SunriseDate(DateTime sunriseDate)
        {
            _sunriseDate = sunriseDate;
            return this;
        }

        public ContentfulNewsBuilder SunsetDate(DateTime sunsetDate)
        {
            _sunsetDate = sunsetDate;
            return this;
        }

        public ContentfulNewsBuilder Body(string body)
        {
            _body = body;
            return this;
        }

        public ContentfulNewsBuilder Document()
        {
            _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
            return this;
        }

        public ContentfulNewsBuilder Tags(List<string> tags)
        {
            _tags = tags;
            return this;
        }
    }
}
