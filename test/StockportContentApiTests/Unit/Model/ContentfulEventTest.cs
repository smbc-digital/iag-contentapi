using System;
using System.Collections.Generic;
using FluentAssertions;
using StockportContentApi.Model;
using Xunit;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulEventTest
    {
        private const string ThumbnailQuery = "?h=250";

        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var anEvent = new ContentfulEvent();
            var expectedEvent = new ContentfulEvent(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false, DateTime.MinValue.ToUniversalTime(),
                                          string.Empty, string.Empty, 0, EventFrequency.None, new List<Crumb> { new Crumb("Events", string.Empty, "events") }, new List<Asset>());

            anEvent.ShouldBeEquivalentTo(expectedEvent);
        }

        [Fact]
        public void ShouldSetImageUrlsOnBuiltEventModel()
        {
            var rawEvent = new ContentfulEvent(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false, DateTime.MinValue.ToUniversalTime(),
                                          string.Empty, string.Empty, 0, EventFrequency.None, new List<Crumb> { new Crumb("Events", string.Empty, "events") } , new List<Asset>() { new Asset { File = new File { Url = "test.img" } } });

            var builtEvent = rawEvent.ToModel();

            builtEvent.ImageUrl.Should().Be(rawEvent.Image.File.Url);
            builtEvent.ThumbnailImageUrl.Should().Be(rawEvent.Image.File.Url + ThumbnailQuery);
        }
    }
}