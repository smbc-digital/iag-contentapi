namespace StockportContentApiTests.Unit.ContentfulFactories;

public class InlineQuoteContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateInlineQuoteFromAContentfulReference()
    {
        // Arrange
        ContentfulInlineQuote contentfulReference = new ContentfulInlineQuoteBuilder().Build();

        // Act
        InlineQuote result = new InlineQuoteContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Image.File.Url, result.Image);
        Assert.Equal(contentfulReference.ImageAltText, result.ImageAltText);
        Assert.Equal(contentfulReference.Quote, result.Quote);
        Assert.Equal(contentfulReference.Author, result.Author);
        Assert.Equal(contentfulReference.Slug, result.Slug);
        Assert.Equal(contentfulReference.Theme, result.Theme);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulInlineQuote contentfulReference = new ContentfulInlineQuoteBuilder()
            .WithImageAltText(string.Empty)
            .WithQuote(string.Empty)
            .WithAuthor(string.Empty)
            .WithSlug(string.Empty)
            .WithTheme(EColourScheme.None)
            .Build();

        // Act
        InlineQuote result = new InlineQuoteContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.ImageAltText);
        Assert.Empty(result.Quote);
        Assert.Empty(result.Author);
        Assert.Empty(result.Slug);
        Assert.Equal(EColourScheme.None, result.Theme);
    }
}