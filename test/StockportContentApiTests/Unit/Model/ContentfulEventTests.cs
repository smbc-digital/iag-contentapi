namespace StockportContentApiTests.Unit.Model;

public class ContentfulEventTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulEvent actual = new();

        ContentfulEvent expected = new()
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