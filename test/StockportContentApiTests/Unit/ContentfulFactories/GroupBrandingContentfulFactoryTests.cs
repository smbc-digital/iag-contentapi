namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupBrandingContentfulFactoryTests
{

    [Fact]
    public void ToModel_ShouldCreateGroupBrandingFromAContentfulGroupBranding()
    {
        // Arrange
        ContentfulGroupBranding contentfulReference = new ContentfulGroupBrandingBuilder().Build();

        // Act
        GroupBranding result = new GroupBrandingContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.Text, result.Text);
        Assert.Equal(contentfulReference.File.File.Url, result.File.Url);
        Assert.Equal(contentfulReference.Url, result.Url);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulGroupBranding contentfulReference = new ContentfulGroupBrandingBuilder()
            .WithTitle(string.Empty)
            .WithText(string.Empty)
            .WithFile(null)
            .WithUrl(string.Empty)
            .Build();

        // Act
        GroupBranding result = new GroupBrandingContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.Text);
        Assert.NotNull(result.File);
        Assert.IsType<MediaAsset>(result.File);
        Assert.Empty(result.Url);
    }
}