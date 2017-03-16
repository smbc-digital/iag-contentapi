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
        private List<Entry<ContentfulProfile>> _profiles = new List<Entry<ContentfulProfile>> {
            new ContentfulEntryBuilder<ContentfulProfile>().Fields(new ContentfulProfileBuilder().Build()).Build() };
        private List<Entry<Alert>> _alertsInline = new List<Entry<Alert>> { new ContentfulEntryBuilder<Alert>().Fields(new Alert("title", "subHeading", "body", "severity",
                                                                                                                       new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                                                                                       new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc))).Build() };

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
                AlertsInline = _alertsInline
            };
        }
    }
}
