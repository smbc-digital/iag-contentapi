namespace StockportContentApiTests.Unit.ContentfulFactories;

public class AlertContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateAlertFromAContentfulAlert()
    {
        // Arrange
        ContentfulAlert contentfulReference = new ContentfulAlertBuilder().Build();

        // Act
        Alert result = new AlertContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, result.Slug);
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.Body, result.Body);
        Assert.Equal(contentfulReference.Severity, result.Severity);
        Assert.Equal(contentfulReference.SunriseDate, result.SunriseDate);
        Assert.Equal(contentfulReference.SunsetDate, result.SunsetDate);
        Assert.Equal(contentfulReference.IsStatic, result.IsStatic);
        Assert.Equal(contentfulReference.Image.File.Url, result.ImageUrl);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulAlert contentfulReference = new ContentfulAlertBuilder()
            .WithSlug(string.Empty)
            .WithTitle(string.Empty)
            .WithBody(string.Empty)
            .WithSeverity(string.Empty)
            .WithSunriseDate(new DateTime())
            .WithSunsetDate(new DateTime())
            .Build();

        // Act
        Alert result = new AlertContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Slug);
        Assert.Empty(result.Body);
        Assert.Empty(result.Severity);
    }
}