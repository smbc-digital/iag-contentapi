namespace StockportContentApiTests.Unit.Model;

public class ContentfulSectionTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulSection actual = new();
        ContentfulSection expected = new()
        {
            Title = string.Empty,
            Slug = string.Empty,
            Body = string.Empty,
            Profiles = new List<ContentfulProfile>(),
            Documents = new List<Asset>(),
            SunriseDate = DateTime.MinValue.ToUniversalTime(),
            SunsetDate = DateTime.MaxValue.ToUniversalTime()
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
