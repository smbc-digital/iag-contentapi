using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulSectionBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _metaDescription = "metaDescription";
        private string _body = "body";
        private DateTime _sunriseDate = DateTime.MinValue;
        private DateTime _sunsetDate = DateTime.MinValue;
        private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
        private List<ContentfulProfile> _profiles = new List<ContentfulProfile> {
            new ContentfulProfileBuilder().Build() };
        private List<ContentfulAlert> _alertsInline = new List<ContentfulAlert>
        {
            new ContentfulAlertBuilder().Build()
        };
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        public ContentfulSection Build()
        {
            return new ContentfulSection
            {
                Title = _title,
                Slug = _slug,
                MetaDescription = _metaDescription,
                Body = _body,
                Profiles = _profiles,
                Documents = _documents,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                AlertsInline = _alertsInline,
                Sys = _sys
            };
        }
    }
}
