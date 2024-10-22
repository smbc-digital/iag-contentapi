namespace StockportContentApiTests.Unit.ContentfulFactories;

public class TopicSiteMapContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateTopicSiteMapFromAContentfulReference()
    {
        // Arrange
        ContentfulTopicForSiteMap contentfulReference = new ContentfulTopicForSiteMapBuilder().Build();

        // Act
        TopicSiteMap result = new TopicSiteMapContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, result.Slug);
        Assert.Equal(contentfulReference.SunriseDate, result.SunriseDate);
        Assert.Equal(contentfulReference.SunsetDate, result.SunsetDate);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulTopicForSiteMap contentfulReference = new ContentfulTopicForSiteMapBuilder()
            .WithSlug(string.Empty)
            .Build();

        // Act
        TopicSiteMap result = new TopicSiteMapContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Slug);
    }
}