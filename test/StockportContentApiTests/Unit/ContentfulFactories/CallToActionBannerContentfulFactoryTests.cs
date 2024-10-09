namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CallToActionBannerContentfulFactoryTests
{
    private readonly CallToActionBannerContentfulFactory _factory = new();

    [Fact]
    public void ToModel_ShouldReturnNull_IfEntryIsNull()
    {
        // Act
        CallToActionBanner result = _factory.ToModel(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToModel_ShouldReturnCallToActionBanner()
    {
        // Arrange
        ContentfulCallToActionBanner model = new()
        {
            AltText = "alt text",
            ButtonText = "button text",
            Image = new Asset
            {
                File = new File
                {
                    Url = "url"
                }
            },
            Link = "link",
            Title = "title",
            Teaser = "teaser"
        };

        // Act
        CallToActionBanner result = _factory.ToModel(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Image);
        Assert.Equal("alt text", result.AltText);
        Assert.Equal("button text", result.ButtonText);
        Assert.Equal("link", result.Link);
        Assert.Equal("title", result.Title);
        Assert.Equal("teaser", result.Teaser);
        Assert.Equal("url", result.Image);
    }

    [Fact]
    public void ToModel_ShouldReturnCallToActionBannerWithNullImage()
    {
        // Arrange
        ContentfulCallToActionBanner model = new()
        {
            AltText = "alt text",
            ButtonText = "button text",
            Link = "link",
            Title = "title",
            Teaser = "teaser"
        };

        // Act
        CallToActionBanner result = _factory.ToModel(model);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Image);
    }
}