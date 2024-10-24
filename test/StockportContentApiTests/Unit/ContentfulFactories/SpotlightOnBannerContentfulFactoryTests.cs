namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SpotlightOnBannerContentfulFactoryTests
{
    private readonly SpotlightOnBannerContentfulFactory _spotlightOnBannerContentfulFactory;

    public SpotlightOnBannerContentfulFactoryTests()
    {
        _spotlightOnBannerContentfulFactory = new SpotlightOnBannerContentfulFactory();
    }

    [Fact]
    public void ToModel_ShouldCreateSpotlightOnListFromAContentfulSpotlightOnList()
    {
        // Arrange
        ContentfulSpotlightOnBanner entry = new()
        {
            Title = "Spotlight title",
            Image = new Asset() {
                File = new File() {
                    Url = "ImageUrl"
                }
            },
            AltText = "AltText",
            Link = "Link",
            Sys = new SystemProperties {
                PublishedAt = DateTime.MinValue,
                UpdatedAt = DateTime.MinValue.AddDays(1)
            }
        };

        // Act
        SpotlightOnBanner result = _spotlightOnBannerContentfulFactory.ToModel(entry);

        // Assert
        Assert.Equal(entry.Title, result.Title);
        Assert.Equal(entry.Image.File.Url, result.Image);
        Assert.Equal(entry.AltText, result.AltText);
        Assert.Equal(entry.Link, result.Link);
        Assert.Equal(entry.Sys.UpdatedAt, result.LastUpdated); ;
    }

    [Fact]
    public void ToModel_LastUpdatedShouldUsePublishDate_If_LastUpdatedNotFound()
    {
        // Arrange
        ContentfulSpotlightOnBanner entry = new()
        {
            Title = "Spotlight title",
            Image = new Asset() {
                File = new File() {
                    Url = "ImageUrl"
                }
            },
            AltText = "AltText",
            Link = "Link",
            Sys = new SystemProperties {
                PublishedAt = DateTime.MinValue
            }
        };

        // Act
        SpotlightOnBanner result = _spotlightOnBannerContentfulFactory.ToModel(entry);

        // Assert
        Assert.Equal(entry.Sys.PublishedAt, result.LastUpdated);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_If_NoEntry()
    {
        // Act
        SpotlightOnBanner result = _spotlightOnBannerContentfulFactory.ToModel(null);

        // Assert
        Assert.Null(result);
    }
}