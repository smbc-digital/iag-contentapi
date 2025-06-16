namespace StockportContentApiTests.Unit.ContentfulFactories;

public class TriviaContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateTriviaFromAContentfulTrivia()
    {
        // Arrange
        ContentfulTrivia contentfulReference = new ContentfulTriviaBuilder().Build();

        // Act
        Trivia result = new TriviaContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.Icon, result.Icon);
        Assert.Equal(contentfulReference.Body, result.Body);
        Assert.Equal(contentfulReference.Link, result.Link);
        Assert.Equal(contentfulReference.Statistic, result.Statistic);
        Assert.Equal(contentfulReference.StatisticSubHeading, result.StatisticSubheading);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulTrivia contentfulReference = new ContentfulTriviaBuilder()
            .WithTitle(string.Empty)
            .WithIcon(string.Empty)
            .WithBody(string.Empty)
            .WithLink(string.Empty)
            .WithStatistic(string.Empty)
            .WithStatisticSubHeading(string.Empty)
            .Build();

        // Act
        Trivia result = new TriviaContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Title);
        Assert.Empty(result.Icon);
        Assert.Empty(result.Body);
        Assert.Empty(result.Link);
        Assert.Empty(result.Statistic);
        Assert.Empty(result.StatisticSubheading);
    }
}