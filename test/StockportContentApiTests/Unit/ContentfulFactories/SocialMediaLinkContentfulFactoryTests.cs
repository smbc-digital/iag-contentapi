namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SocialMediaLinkContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateSocialMediaLinkFromAContentfulSocialMediaLink()
    {
        // Arrange
        ContentfulSocialMediaLink contentfulReference = new ContentfulSocialMediaLinkBuilder().Build();

        // Act
        SocialMediaLink result = new SocialMediaLinkContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.Slug, result.Slug);
        Assert.Equal(contentfulReference.Link, result.Link);
        Assert.Equal(contentfulReference.Icon, result.Icon);
        Assert.Equal(contentfulReference.AccountName, result.AccountName);
        Assert.Equal(contentfulReference.ScreenReader, result.ScreenReader);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulSocialMediaLink contentfulReference = new ContentfulSocialMediaLinkBuilder()
            .WithTitle(string.Empty)
            .WithSlug(string.Empty)
            .WithLink(string.Empty)
            .WithIcon(string.Empty)
            .WithAccountName(string.Empty)
            .WithScreenReader(string.Empty)
            .Build();

        // Act
        SocialMediaLink result = new SocialMediaLinkContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.Slug);
        Assert.Empty(result.Link);
        Assert.Empty(result.Icon);
        Assert.Empty(result.AccountName);
        Assert.Empty(result.ScreenReader);
    }
}