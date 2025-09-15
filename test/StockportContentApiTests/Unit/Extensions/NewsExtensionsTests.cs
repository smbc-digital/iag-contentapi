namespace StockportContentApiTests.Unit.Extensions;

public class NewsExtensionsTests
{
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public NewsExtensionsTests() =>
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 12, 07));

    [Fact]
    public void ShouldReturnTwoDatesForNewsItems()
    {
        // Arrange
        List<News> news = new()
        {
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "hero image caption",
                    "body",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "hero image caption",
                    "body",
                    new DateTime(2016, 03, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty)
        };

        List<DateTime> dates;

        // Act
        IEnumerable<News> result = news.GetNewsDates(out dates, _timeProvider.Object);

        // Assert
        Assert.Equal(2, dates.Count);
        Assert.Equal(2, dates[0].Month);
        Assert.Equal(2016, dates[0].Year);
        Assert.Equal(3, dates[1].Month);
        Assert.Equal(2016, dates[1].Year);
        Assert.Equivalent(result, news);
    }

    [Fact]
    public void ShouldReturnOneDateForDuplicateMonths()
    {
        // Arrange
        List<News> news = new()
        {
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty)
        };

        List<DateTime> dates;

        // Act
        IEnumerable<News> result = news.GetNewsDates(out dates, _timeProvider.Object);

        // Assert
        Assert.Single(dates);
        Assert.Equal(2, dates[0].Month);
        Assert.Equal(2016, dates[0].Year);
        Assert.Equivalent(result, news);
    }

    [Fact]
    public void ShouldReturnOnlyReturnCurrentAndPastNewsItems()
    {
        // Arrange
        List<News> news = new()
        {
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 03, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 06, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 12, 10),
                    new DateTime(2016, 12, 25),
                    new DateTime(2016, 05, 01),
                    new List<Alert>(),
                    new List<string>(),
                    new List<string>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty)
        };

        List<DateTime> dates;

        // Act
        IEnumerable<News> result = news.GetNewsDates(out dates, _timeProvider.Object);

        // Assert
        Assert.Equal(3, dates.Count);
        Assert.Equal(2016, dates[1].Year);
        Assert.Equal(3, dates[1].Month);
        Assert.Equivalent(result, news);
    }
}
