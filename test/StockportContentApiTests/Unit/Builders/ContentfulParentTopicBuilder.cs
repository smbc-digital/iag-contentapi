﻿using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulParentTopicBuilder
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
            
        public ContentfulTopic Build()
        {
            return new ContentfulTopic
            {
                Name = _name,               
                SubItems = _subItems,
                SecondaryItems = _secondaryItems,
                TertiaryItems  = _tertiaryItems,              
            };
        }
    }
}