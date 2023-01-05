using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using Xunit;

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
                Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
                Description = string.Empty,
                Fee = string.Empty,
                Location = string.Empty,
                SubmittedBy = string.Empty,
                EventDate = DateTime.MinValue.ToUniversalTime(),
                StartTime = string.Empty,
                EndTime = string.Empty,
                Occurences = 0,
                Frequency = EventFrequency.None,
                Documents = new List<Asset>(),
                MapPosition = new MapPosition(),
                BookingInformation = string.Empty,
                Featured = false,
                Sys = new SystemProperties(),
                Tags = new List<string>(),
                Alerts = new List<ContentfulAlert>()
            };
            actual.Should().BeEquivalentTo(expected);
        }
    }
}