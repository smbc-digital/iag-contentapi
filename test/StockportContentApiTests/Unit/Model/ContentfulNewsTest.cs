namespace StockportContentApiTests.Unit.Model;

public class ContentfulNewsTest
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        var actual = new ContentfulNews();
        var expected = new ContentfulNews
        {
            Alerts = new List<ContentfulAlert>(),
            Body = string.Empty,
            Categories = new List<string>(),
            Documents = new List<Asset>(),
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            Slug = string.Empty,
            SunriseDate = DateTime.MinValue.ToUniversalTime(),
            SunsetDate = DateTime.MaxValue.ToUniversalTime(),
            Tags = new List<string>(),
            Teaser = string.Empty,
            Title = string.Empty
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
