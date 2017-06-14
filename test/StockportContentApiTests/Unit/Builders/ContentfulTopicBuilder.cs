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
        private string _expandingLinkTitle = "expandingLinkTitle";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private Asset _backgroundImage = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
        private Asset _image = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
        private List<Entry<ContentfulCrumb>> _breadcrumbs = new List<Entry<ContentfulCrumb>> {
            new ContentfulEntryBuilder<ContentfulCrumb>().Fields(new ContentfulCrumbBuilder().Build()).Build() };
        private bool _emailAlerts = false;
        private string _emailAlertsTopicId = "id";
        private List<Entry<ContentfulAlert>> _alerts = new List<Entry<ContentfulAlert>> {
            new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().Build()).Build()};
        private List<Entry<ContentfulSubItem>> _subItems = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("sub-slug").Build()).Build() };
        private List<Entry<ContentfulSubItem>> _secondaryItems = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("secondary-slug").Build()).Build() };
        private List<Entry<ContentfulSubItem>> _tertiaryItems = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("tertiary-slug").Build()).Build() };
        private Entry<ContentfulEventBanner> _eventBanner =
            new ContentfulEntryBuilder<ContentfulEventBanner>().Fields(new ContentfulEventBannerBuilder().Build()).Build();
        private List<Entry<ContentfulExpandingLinkBox>> _expandingLinkBox = new List<Entry<ContentfulExpandingLinkBox>> {
            new ContentfulEntryBuilder<ContentfulExpandingLinkBox>().Fields(new ContentfulExpandingLinkBoxBuilder().Title("title").Build()).Build() };

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
                ExpandingLinkTitle = _expandingLinkTitle,
                ExpandingLinkBoxes = _expandingLinkBox
            };
        }

        public ContentfulTopicBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulTopicBuilder Alerts(List<Entry<ContentfulAlert>> alerts)
        {
            _alerts = alerts;
            return this;
        }
    }
}
