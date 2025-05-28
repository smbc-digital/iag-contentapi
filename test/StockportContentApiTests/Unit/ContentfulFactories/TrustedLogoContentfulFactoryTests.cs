namespace StockportContentApiTests.Unit.ContentfulFactories;

public class TrustedLogoContentfulFactoryTests
{

    [Fact]
    public void ToModel_ShouldCreateTrustedLogoFromAContentfulTrustedLogo()
    {
        // Arrange
        ContentfulTrustedLogo contentfulReference = new ContentfuTrustedLogoBuilder().Build();

        // Act
        TrustedLogo result = new TrustedLogoContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.Text, result.Text);
        Assert.Equal(contentfulReference.Image.File.Url, result.Image.Url);
        Assert.Equal(contentfulReference.Link, result.Link);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulTrustedLogo contentfulReference = new ContentfuTrustedLogoBuilder()
            .WithTitle(string.Empty)
            .WithText(string.Empty)
            .WithFile(null)
            .WithUrl(string.Empty)
            .Build();

        // Act
        TrustedLogo result = new TrustedLogoContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.Text);
        Assert.NotNull(result.Image);
        Assert.IsType<MediaAsset>(result.Image);
        Assert.Empty(result.Link);
    }
}