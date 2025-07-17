namespace StockportContentApiTests.Unit.Model;

public class ContentfulNewsTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulNews actual = new();
        ContentfulNews expected = new()
        {
            Alerts = new List<ContentfulAlert>(),
            Body = string.Empty,
            Categories = new List<string>(),
            Documents = new List<Asset>(),
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            Slug = string.Empty,
            SunriseDate = DateTime.MinValue.ToString("o"),
            SunsetDate = DateTime.MaxValue,
            Tags = new List<string>(),
            Teaser = string.Empty,
            Title = string.Empty,
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
