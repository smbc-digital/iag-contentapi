using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulArticleTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulArticle();
            var expected = new ContentfulArticle
            {
                Body = string.Empty,
                Slug = string.Empty,
                Title = string.Empty,
                Teaser = string.Empty,
                Icon = string.Empty,
                BackgroundImage = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
                Sections = new List<ContentfulSection>(),
                Breadcrumbs = new List<ContentfulReference>(),
                Alerts = new List<ContentfulAlert>(),
                Profiles = new List<ContentfulProfile>(),
                Documents = new List<Asset>(),
                SunriseDate = DateTime.MinValue,
                SunsetDate = DateTime.MaxValue,
                LiveChatVisible = false,
                LiveChatText = new ContentfulLiveChat()
            };
            actual.Should().BeEquivalentTo(expected);
        }
    }
}