using System;
using System.Collections.Generic;
using FluentAssertions;
using StockportContentApi.Model;
using Xunit;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulEventTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulEvent();
            var expected = new ContentfulEvent
            {
                Title = string.Empty,
                Slug = string.Empty,
                Teaser = string.Empty,
                Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" }},
                Description = string.Empty,
                Fee = string.Empty,
                Location = string.Empty,
                SubmittedBy = string.Empty,
                EventDate = DateTime.MinValue.ToUniversalTime(),
                StartTime = string.Empty,
                EndTime = string.Empty,
                Occurences = 0,
                Frequency = EventFrequency.None,
                Breadcrumbs = new List<Crumb> { new Crumb("Events", string.Empty, "events") },
                Documents = new List<Asset>(),
                MapPosition = new MapPosition()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}