namespace StockportContentApiTests.Unit.Model;

public class ContentfulTopicTest
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        var actual = new ContentfulTopic();
        var expected = new ContentfulTopic
        {
            Slug = string.Empty,
            Name = string.Empty,
            Teaser = string.Empty,
            Summary = string.Empty,
            Icon = string.Empty,
            BackgroundImage = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            SubItems = new List<ContentfulReference>(),
            SecondaryItems = new List<ContentfulReference>(),
            TertiaryItems = new List<ContentfulReference>(),
            Breadcrumbs = new List<ContentfulReference>(),
            Alerts = new List<ContentfulAlert>(),
            SunriseDate = DateTime.MinValue.ToUniversalTime(),
            SunsetDate = DateTime.MaxValue.ToUniversalTime(),
            EmailAlerts = false,
            EmailAlertsTopicId = string.Empty
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
