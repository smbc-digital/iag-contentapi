namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CarouselContentContentfulFactoryTests
{
    private readonly CarouselContentContentfulFactory _factory = new();

    [Fact]
    public void ToModel_ShouldReturnCarouselContent()
    {
        // Arrange
        ContentfulCarouselContent model = new()
        {
            Slug = "slug",
            Url = "url",
            Image = new Asset
            {
                File = new File
                {
                    Url = "imageUrl"
                },
                SystemProperties = new SystemProperties()
            },
            SunriseDate = DateTime.MinValue,
            SunsetDate = DateTime.MaxValue,
            Title = "title",
            Teaser = "teaser"
        };

        // Act
        CarouselContent result = _factory.ToModel(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Image);
        Assert.Equal(model.Slug, result.Slug);
        Assert.Equal(model.Url, result.Url);
        Assert.Equal(DateComparer.DateFieldToDate(model.SunriseDate), result.SunriseDate);
        Assert.Equal(DateComparer.DateFieldToDate(model.SunsetDate), result.SunsetDate);
        Assert.Equal(model.Title, result.Title);
        Assert.Equal(model.Teaser, result.Teaser);
        Assert.Equal(model.Image.File.Url, result.Image);
    }

    [Fact]
    public void ToModel_ShouldReturnCallToActionBannerWithEmptyStrings()
    {
        // Arrange
        ContentfulCarouselContent model = new()
        {
            SunriseDate = DateTime.MinValue,
            SunsetDate = DateTime.MaxValue,
        };

        // Act
        CarouselContent result = _factory.ToModel(model);

        // Assert
        Assert.Equal(string.Empty, result.Slug);
        Assert.Equal(string.Empty, result.Url);
        Assert.Equal(string.Empty, result.Title);
        Assert.Equal(string.Empty, result.Teaser);
        Assert.Equal(string.Empty, result.Image);
    }
}