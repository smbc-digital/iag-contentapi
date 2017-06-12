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
        private Asset _backgroundImage = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
        private Asset _image = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
        private List<ContentfulCrumb> _breadcrumbs = new List<ContentfulCrumb> {
            new ContentfulCrumbBuilder().ContentTypeSystemId("topic").Build() };
        private bool _emailAlerts = false;
        private string _emailAlertsTopicId = "id";
        private List<ContentfulAlert> _alerts = new List<ContentfulAlert> {
            new ContentfulAlertBuilder().Build()};
        private List<IContentfulSubItem> _subItems = new List<IContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("sub-slug").Build() };
        private List<IContentfulSubItem> _secondaryItems = new List<IContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("secondary-slug").Build() };
        private List<IContentfulSubItem> _tertiaryItems = new List<IContentfulSubItem> {
           new ContentfulSubItemBuilder().Slug("tertiary-slug").Build() };
        private ContentfulEventBanner _eventBanner =
           new ContentfulEventBannerBuilder().Build();
        private string _systemId = "id";
        private string _contentTypeSystemId = "id";

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
                Image = _image,
                SubItems = _subItems,
                SecondaryItems = _secondaryItems,
                TertiaryItems  = _tertiaryItems,
                Breadcrumbs  = _breadcrumbs,
                Alerts = _alerts,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                EmailAlerts  = _emailAlerts,
                EmailAlertsTopicId  = _emailAlertsTopicId,
                EventBanner = _eventBanner,
                Sys = new SystemProperties
                {
                    ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                    Id = _systemId
                }
            };
        }

        public ContentfulTopicBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulTopicBuilder Alerts(List<ContentfulAlert> alerts)
        {
            _alerts = alerts;
            return this;
        }

        public ContentfulTopicBuilder SystemId(string id)
        {
            _systemId = id;
            return this;
        }

        public ContentfulTopicBuilder SystemContentTypeId(string id)
        {
            _contentTypeSystemId = id;
            return this;
        }

        public ContentfulTopicBuilder Breadcrumbs(List<ContentfulCrumb> crumb)
        {
            _breadcrumbs = crumb;
            return this;
        }
    }
}
