using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulTopicTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulTopic();
            var expected = new ContentfulTopic
            {
                Slug = string.Empty,
                Name = string.Empty,
                Teaser = string.Empty,
                Summary = string.Empty,
                Icon = string.Empty,
                BackgroundImage = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
                SubItems = new List<Entry<ContentfulSubItem>>(),
                SecondaryItems = new List<Entry<ContentfulSubItem>>(),
                TertiaryItems = new List<Entry<ContentfulSubItem>>(),
                Breadcrumbs = new List<Entry<ContentfulCrumb>>(),
                Alerts = new List<Entry<Alert>>(),
                SunriseDate = DateTime.MinValue.ToUniversalTime(),
                SunsetDate = DateTime.MaxValue.ToUniversalTime(),
                EmailAlerts = false,
                EmailAlertsTopicId = string.Empty
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
