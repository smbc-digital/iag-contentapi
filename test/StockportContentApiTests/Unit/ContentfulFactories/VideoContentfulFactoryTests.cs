namespace StockportContentApiTests.Unit.ContentfulFactories;

public class VideoContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateVideoFromAContentfulVideo()
    {
        // Arrange
        ContentfulVideo contentfulReference = new ContentfulVideoBuilder().Build();

        // Act
        Video result = new VideoContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Heading, result.Heading);
        Assert.Equal(contentfulReference.Text, result.Text);
        Assert.Equal(contentfulReference.VideoEmbedCode, result.VideoEmbedCode);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulVideo contentfulReference = new ContentfulVideoBuilder()
            .WithHeading(string.Empty)
            .WithText(string.Empty)
            .WithVideoEmbedCode(string.Empty)
            .Build();

        // Act
        Video result = new VideoContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Heading);
        Assert.Empty(result.Text);
        Assert.Empty(result.VideoEmbedCode);
    }
}