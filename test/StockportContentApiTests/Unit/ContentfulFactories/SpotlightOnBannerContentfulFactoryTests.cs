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
        List<ContentfulSpotlightOnBanner> entry = new()
        {
            new ContentfulSpotlightOnBanner {
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
            }
        };

        // Act
        IEnumerable<SpotlightOnBanner> result = _spotlightOnBannerContentfulFactory.ToModel(entry);

        // Assert
        Assert.Single(result);
        Assert.Equal(entry.First().Title, result.First().Title);
        Assert.Equal(entry.First().Image.File.Url, result.First().Image);
        Assert.Equal(entry.First().AltText, result.First().AltText);
        Assert.Equal(entry.First().Link, result.First().Link);
        Assert.Equal(entry.First().Sys.UpdatedAt, result.First().LastUpdated); ;
    }

    [Fact]
    public void ToModel_LastUpdatedShouldUsePublishDate_If_LastUpdatedNotFound()
    {
        // Arrange
        List<ContentfulSpotlightOnBanner> entry = new()
        {
            new ContentfulSpotlightOnBanner {
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
            }
        };

        // Act
        IEnumerable<SpotlightOnBanner> result = _spotlightOnBannerContentfulFactory.ToModel(entry);

        // Assert
        Assert.Equal(entry.First().Sys.PublishedAt, result.First().LastUpdated);
    }

    [Fact]
    public void ToModel_ShouldReturnEmptyList_If_ContentfulListIsNull()
    {
        // Act
        IEnumerable<SpotlightOnBanner> result = _spotlightOnBannerContentfulFactory.ToModel(null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}