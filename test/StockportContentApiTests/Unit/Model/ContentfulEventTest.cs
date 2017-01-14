using System;
using System.Collections.Generic;
using FluentAssertions;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulEventTest
    {
        private const string ThumbnailQuery = "?h=250";

        [Fact]
        public void ShouldSetImageUrlsOnBuiltEventModel()
        {
            const string imageUrl = "//image-url";
            var rawEvent = new ContentfulEvent(string.Empty, string.Empty, string.Empty, imageUrl, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false, DateTime.MinValue.ToUniversalTime(),
                                          string.Empty, string.Empty, 0, EventFrequency.None, new List<Crumb> { new Crumb("Events", string.Empty, "events") });

            var builtEvent = rawEvent.ToModel();

            builtEvent.ImageUrl.Should().Be(imageUrl);
            builtEvent.ThumbnailImageUrl.Should().Be(imageUrl + ThumbnailQuery);
        }
    }
}