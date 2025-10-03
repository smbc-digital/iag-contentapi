namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SpotlightOnBannerContentfulFactoryTests
{
    private readonly SpotlightOnBannerContentfulFactory _spotlightOnBannerFactory;

    public SpotlightOnBannerContentfulFactoryTests() =>
        _spotlightOnBannerFactory = new SpotlightOnBannerContentfulFactory();

    [Fact]
    public void ToModel_ShouldCreateSpotlightOnListFromAContentfulSpotlightOnList()
    {
        // Arrange
        ContentfulSpotlightOnBanner contentfulSpotlightOnBanner = new ContentfulSpotlightOnBannerBuilder().Build();

        // Act
        SpotlightOnBanner result = _spotlightOnBannerFactory.ToModel(contentfulSpotlightOnBanner);

        // Assert
        Assert.Equal(contentfulSpotlightOnBanner.Title, result.Title);
        Assert.Equal(contentfulSpotlightOnBanner.Image.File.Url, result.Image.Url);
        Assert.Equal(contentfulSpotlightOnBanner.Link, result.Link);
        Assert.Equal(contentfulSpotlightOnBanner.Sys.UpdatedAt, result.LastUpdated); ;
    }

    [Fact]
    public void ToModel_LastUpdatedShouldUsePublishDate_If_LastUpdatedNotFound()
    {
        // Arrange
        ContentfulSpotlightOnBanner contentfulSpotlightOnBanner = new ContentfulSpotlightOnBannerBuilder().Build();
        contentfulSpotlightOnBanner.Sys.UpdatedAt = null;

        // Act
        SpotlightOnBanner result = _spotlightOnBannerFactory.ToModel(contentfulSpotlightOnBanner);

        // Assert
        Assert.Equal(contentfulSpotlightOnBanner.Sys.PublishedAt, result.LastUpdated);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_If_NoEntry()
    {
        // Act
        SpotlightOnBanner result = _spotlightOnBannerFactory.ToModel(null);

        // Assert
        Assert.Null(result);
    }
}