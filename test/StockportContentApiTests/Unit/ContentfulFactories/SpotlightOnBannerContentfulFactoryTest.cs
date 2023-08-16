using System.Drawing;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SpotlightOnBannerContentfulFactoryTest
{
    private readonly SpotlightOnBannerContentfulFactory _spotlightOnBannerContentfulFactory;

    public SpotlightOnBannerContentfulFactoryTest() {
        _spotlightOnBannerContentfulFactory = new SpotlightOnBannerContentfulFactory();
    }

    [Fact]
    public void ShouldCreateSpotlightOnListFromAContentfulSpotlightOnList()
    {
        List<ContentfulSpotlightOnBanner> entry = new List<ContentfulSpotlightOnBanner>{
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

        var result = _spotlightOnBannerContentfulFactory.ToModel(entry);

        result.Count().Should().Be(1);
        result.First().Title.Should().Be(entry.First().Title);
        result.First().Image.Should().Be(entry.First().Image.File.Url);
        result.First().AltText.Should().Be(entry.First().AltText);
        result.First().Link.Should().Be(entry.First().Link);
        result.First().LastUpdated.Should().Be(entry.First().Sys.UpdatedAt);
    }

    [Fact]
    public void LastUpdatedShouldUsePublishDateIfLastUpdatedNotFound()
    {
        List<ContentfulSpotlightOnBanner> entry = new List<ContentfulSpotlightOnBanner>{
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

        var result = _spotlightOnBannerContentfulFactory.ToModel(entry);

        result.First().LastUpdated.Should().Be(entry.First().Sys.PublishedAt);
    }

    [Fact]
    public void ToModelShouldReturnEmptyListIfContentfulListIsNull()
    {
        var result = _spotlightOnBannerContentfulFactory.ToModel(null);

        result.Should().NotBeNull();
        result.Count().Should().Be(0);
    }
}
