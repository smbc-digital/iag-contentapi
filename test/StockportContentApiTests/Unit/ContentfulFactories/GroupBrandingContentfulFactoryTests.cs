﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupBrandingContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateGroupBrandingFromAContentfulGroupBranding()
    {
        // Arrange
        ContentfulTrustedLogos contentfulReference = new ContentfulTrustedLogosBuilder().Build();

        // Act
        TrustedLogos result = new GroupBrandingContentfulFactory().ToModel(contentfulReference);

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
        ContentfulTrustedLogos contentfulReference = new ContentfulTrustedLogosBuilder()
            .WithTitle(string.Empty)
            .WithText(string.Empty)
            .WithFile(null)
            .WithUrl(string.Empty)
            .Build();

        // Act
        TrustedLogos result = new GroupBrandingContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.Text);
        Assert.NotNull(result.File);
        Assert.IsType<MediaAsset>(result.File);
        Assert.Empty(result.Url);
    }
}