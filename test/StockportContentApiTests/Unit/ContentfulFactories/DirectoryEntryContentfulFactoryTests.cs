using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class DirectoryEntryContentfulFactoryTests
{
    private readonly DirectoryEntryContentfulFactory _factory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _brandingFactory = new();
    private readonly ITimeProvider _timeProvider = new TimeProvider();

    public DirectoryEntryContentfulFactoryTests() =>
        _factory = new(_alertFactory.Object, _brandingFactory.Object, _timeProvider);

    [Fact]
    public void ToModel_ShouldCreateADirectorEntryFromAContentfulReference()
    {
        // Arrange
        ContentfulDirectoryEntry entry = new DirectoryEntryBuilder()
            .WithFilter("test-filter", "Test Filter", "Test Filter Display", "test-theme")
            .WithFilter("test-filter A", "Test Filter A", "Test Filter A Display", "test-theme")
            .WithFilter("test-filter2", "Test Filter 2", "Test Filter 2 Display", "test-theme2")
            .Build();

        // Act
        DirectoryEntry result = _factory.ToModel(entry);

        // Assert
        Assert.Equal(entry.Slug, result.Slug);
        Assert.Equal(entry.Name, result.Name);
        Assert.Equal(entry.Provider, result.Provider);
        Assert.Equal(entry.Teaser, result.Teaser);
        Assert.Equal(entry.Description, result.Description);
        Assert.Equal(entry.MetaDescription, result.MetaDescription);
        Assert.Equal(entry.PhoneNumber, result.PhoneNumber);
        Assert.Equal(entry.Email, result.Email);
        Assert.Equal(entry.Website, result.Website);
        Assert.Equal(entry.Twitter, result.Twitter);
        Assert.Equal(entry.Facebook, result.Facebook);
        Assert.Equal(entry.Address, result.Address);
        Assert.Equal(entry.MapPosition.Lat, result.MapPosition.Lat);
        Assert.Equal(entry.MapPosition.Lon, result.MapPosition.Lon);
        Assert.Equal(2, result.Themes.Count());
        Assert.Single(result.Alerts);
        Assert.Single(result.TrustedLogos);
    }

    [Fact]
    public void ToModel_ShouldNotAddAlerts_IfTheyAreLinks()
    {
        // Arrange
        ContentfulDirectoryEntry entry = new DirectoryEntryBuilder()
            .WithFilter("test-filter", "Test Filter", "Test Filter Display", "test-theme")
            .WithFilter("test-filter A", "Test Filter A", "Test Filter A Display", "test-theme")
            .WithFilter("test-filter2", "Test Filter 2", "Test Filter 2 Display", "test-theme2")
            .Build();
        
        entry.Alerts.First().Sys.LinkType = "Link";
        entry.AlertsInline.First().Sys.LinkType = "Link";

        // Act
        DirectoryEntry result = _factory.ToModel(entry);

        // Assert
        Assert.Empty(result.Alerts);
        Assert.Single(result.AlertsInline);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()), Times.Once);
    }
}