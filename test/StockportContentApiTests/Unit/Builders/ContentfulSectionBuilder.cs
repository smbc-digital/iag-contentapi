using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulSectionBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _body = "body";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
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
