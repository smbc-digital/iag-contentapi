namespace StockportContentApiTests.Unit.ContentfulFactories;

public class RedirectContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateRedirectFromAContentfulRedirect()
    {
        // Arrange
        ContentfulRedirect contentfulReference = new ContentfulRedirectBuilder().Build();

        // Act
        BusinessIdToRedirects result = new RedirectContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Redirects, result.ShortUrlRedirects);
        Assert.Equal(contentfulReference.LegacyUrls, result.LegacyUrlRedirects);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulRedirect contentfulReference = new ContentfulRedirectBuilder()
            .WithTitle(string.Empty)
            .WithRedirects(new Dictionary<string, string>())
            .WithLegacyUrls(new Dictionary<string, string>())
            .Build();

        // Act
        BusinessIdToRedirects result = new RedirectContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.ShortUrlRedirects);
        Assert.Empty(result.LegacyUrlRedirects);
    }
}