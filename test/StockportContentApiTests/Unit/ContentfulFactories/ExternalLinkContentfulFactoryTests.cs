namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ExternalLinkContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateExternalLinkFromAContentfulReference()
    {
        // Arrange
        ContentfulExternalLink contentfulReference = new ContentfulExternalLinkBuilder().Build();

        // Act
        ExternalLink result = new ExternalLinkContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.URL, result.URL);
        Assert.Equal(contentfulReference.Teaser, result.Teaser);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulExternalLink contentfulReference = new ContentfulExternalLinkBuilder()
            .WithTitle(string.Empty)
            .WithURL(string.Empty)
            .WithTeaser(string.Empty)
            .Build();

        // Act
        ExternalLink result = new ExternalLinkContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.URL);
        Assert.Empty(result.Teaser);
    }
}