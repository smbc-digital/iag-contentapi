using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulNewsRoomBuilder
    {
        private string _title = "title";
        private List<Alert> _alerts = new List<Alert> { new Alert("title", "subHeading", "body",
                    "severity", new DateTime(2016, 08, 05, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2016, 08, 11, 0, 0, 0, DateTimeKind.Utc)) };

        private bool _emailAlerts = true;
        private string _emailAlertsTopicId = "test-id";
        public SystemProperties Sys { get; set; } = null;

        public ContentfulNewsRoom Build()
        {
            return new ContentfulNewsRoom
            {
                Title = _title,
                Alerts  = _alerts,
                EmailAlertsTopicId = _emailAlertsTopicId,
                EmailAlerts = _emailAlerts,
                Sys = Sys
            };
        }
    }
}
