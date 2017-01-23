using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulTopicBuilder
    {
        private string _name = "name";
        private string _slug = "slug";
        private string _icon = "icon";
        private string _summary = "summary";
        private string _teaser = "teaser";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private Asset _backgroundImage = new Asset { File = new File { Url = "background-image-url.jpg" } };
        private List<Entry<ContentfulCrumb>> _breadcrumbs = new List<Entry<ContentfulCrumb>>
        {
            new Entry<ContentfulCrumb>() {Fields = new ContentfulCrumbBuilder().Build(),
                SystemProperties = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }}
        };
        private bool _emailAlerts = false;
        private string _emailAlertsTopicId = "id";
        private List<Alert> _alerts = new List<Alert> {new Alert("title", "subHeading", "body",
                                                                 "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                                 new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)) };
        private List<Entry<ContentfulSubItem>> _subItems = new List<Entry<ContentfulSubItem>> { new Entry<ContentfulSubItem>
        {
            Fields = new ContentfulSubItem { Icon = "icon", Slug = "slug",
                SunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), Title = "title", Teaser = "teaser"}
        } };

        public ContentfulTopic Build()
        {
            return new ContentfulTopic
            {
                Slug =  _slug,
                Name = _name,
                Teaser = _teaser,
                Summary = _summary,
                Icon = _icon,
                BackgroundImage = _backgroundImage,
                SubItems = _subItems,
                SecondaryItems = _subItems,
                TertiaryItems  = _subItems,
                Breadcrumbs  = _breadcrumbs,
                Alerts = _alerts,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                EmailAlerts  = _emailAlerts,
                EmailAlertsTopicId  = _emailAlertsTopicId
            };
        }
    }
}
