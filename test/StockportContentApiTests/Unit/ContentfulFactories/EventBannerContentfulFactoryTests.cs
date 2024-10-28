namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventBannerContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateEventBannerFromAContentfulEventBanner()
    {
        // Arrange
        ContentfulEventBanner contentfulReference = new ContentfulEventBannerBuilder().Build();

        // Act
        EventBanner result = new EventBannerContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.Teaser, result.Teaser);
        Assert.Equal(contentfulReference.Icon, result.Icon);
        Assert.Equal(contentfulReference.Link, result.Link);
        Assert.Equal(contentfulReference.Colour, result.Colour);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulEventBanner contentfulReference = new ContentfulEventBannerBuilder()
            .WithTitle(string.Empty)
            .WithTeaser(string.Empty)
            .WithLink(string.Empty)
            .WithIcon(string.Empty)
            .Build();

        // Act
        EventBanner result = new EventBannerContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.Teaser);
        Assert.Empty(result.Link);
        Assert.Empty(result.Icon);
    }
}