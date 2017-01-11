﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class EventTest
    {
        private const string ThumbnailQuery = "?h=250";

        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var anEvent = new Event();
            var expectedEvent = new Event(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
                                          DateTime.MinValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), string.Empty, 
                                          string.Empty, string.Empty, string.Empty, string.Empty, false , DateTime.MinValue.ToUniversalTime(), 
                                          string.Empty, string.Empty, 0, EventFrequency.None, new List<Crumb> { new Crumb("Events", string.Empty, "events") });

            anEvent.ShouldBeEquivalentTo(expectedEvent);
        }

        [Fact]
        public void ShouldSetImageUrl()
        {
            const string imageUrl = "//image-url";
            var expectedEvent = new Event(string.Empty, string.Empty, string.Empty, imageUrl, string.Empty,
                                          DateTime.MinValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false, DateTime.MinValue.ToUniversalTime(),
                                          string.Empty, string.Empty, 0, EventFrequency.None, new List<Crumb> { new Crumb("Events", string.Empty, "events") });

            expectedEvent.ImageUrl.Should().Be(imageUrl);
            expectedEvent.ThumbnailImageUrl.Should().Be(imageUrl + ThumbnailQuery);
        }
    }
}