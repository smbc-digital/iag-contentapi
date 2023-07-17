namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CallToActionBannerContentfulFactoryTest
{
    private readonly CallToActionBannerContentfulFactory _factory= new CallToActionBannerContentfulFactory();
    
    [Fact]
    public void ToModel_ShouldReturnNull_IfEntryIsNull()
    {
        // Act
        var result = _factory.ToModel(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToModel_ShouldReturnCallToActionBanner()
    {
        // Arrange
        var model = new ContentfulCallToActionBanner
        {
            AltText = "alt text",
            ButtonText = "button text",
            Image = new Asset(){
                File = new File(){
                    Url = "url"
                }
            },
            Link = "link",
            Title = "title",
            Teaser = "teaser"
        };

        // Act
        var result = _factory.ToModel(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Image);
        Assert.Equal("alt text", result.AltText);
        Assert.Equal("button text", result.ButtonText);
        Assert.Equal("link", result.Link);
        Assert.Equal("title", result.Title);
        Assert.Equal("teaser", result.Teaser);
    }
}
