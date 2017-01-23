using System;
using System.Collections.Generic;
using FluentAssertions;
using StockportContentApi.Model;
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
                BackgroundImage = new Asset { File = new File { Url = string.Empty } },
                Sections = new List<ContentfulSection>(),
                Breadcrumbs = new List<Entry<ContentfulCrumb>>(),
                Alerts = new List<Alert>(),
                Profiles = new List<ContentfulProfile>(),
                ParentTopic = new ContentfulTopic(),
                Documents = new List<Asset>(),
                SunriseDate = DateTime.MinValue,
                SunsetDate = DateTime.MaxValue,
                LiveChatVisible = false,
                LiveChat = new NullLiveChat()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}