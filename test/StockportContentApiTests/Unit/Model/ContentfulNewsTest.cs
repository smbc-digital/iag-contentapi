using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulNewsTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulNews();
            var expected = new ContentfulNews()
            {
                Alerts = new List<Alert>(),
                Body = string.Empty,
                Breadcrumbs = new List<Crumb> { new Crumb("News", string.Empty, "news")},
                Categories = new List<string>(),
                Documents = new List<Asset>(),
                Image = new Asset { File = new File { Url = string.Empty }},
                Slug = string.Empty,
                SunriseDate = DateTime.MinValue.ToUniversalTime(),
                SunsetDate = DateTime.MaxValue.ToUniversalTime(),
                Tags = new List<string>(),
                Teaser = string.Empty,
                Title = string.Empty
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
