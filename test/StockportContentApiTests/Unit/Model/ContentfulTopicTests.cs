namespace StockportContentApiTests.Unit.Model;

public class ContentfulTopicTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        // Arrange & Act
        ContentfulTopic actual = new();
        ContentfulTopic expected = new()
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
            Breadcrumbs = new List<ContentfulReference>(),
            Alerts = new List<ContentfulAlert>(),
            SunriseDate = DateTime.MinValue.ToUniversalTime(),
            SunsetDate = DateTime.MaxValue.ToUniversalTime()
        };

        // Assert
        Assert.Equivalent(expected, actual);
    }
}