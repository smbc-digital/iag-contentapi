namespace StockportContentApiTests.Unit.Model;

public class ContentfulGroupTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulGroup actual = new();
        ContentfulGroup expected = new()
        {
            Name = string.Empty,
            Slug = string.Empty,
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Website = string.Empty,
            Twitter = string.Empty,
            Facebook = string.Empty,
            Address = string.Empty,
            Description = string.Empty,
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            MapPosition = new MapPosition(),
            Volunteering = false
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
