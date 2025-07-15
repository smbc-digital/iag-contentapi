namespace StockportContentApiTests.Unit.Extensions;

public class NewsExtensionsTests
{
    private readonly Mock<ITimeProvider> _timeProvider;

    public NewsExtensionsTests()
    {
        _timeProvider = new Mock<ITimeProvider>();
        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 07));
    }

    [Fact]
    public void ShouldReturnTwoDatesForNewsItems()
    {
        List<News> news = new()
        {
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image",
                    "thumbnail",
                    "hero image caption",
                    "body",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image",
                    "thumbnail",
                    "hero image caption",
                    "body",
                    new DateTime(2016, 03, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty)
        };

        List<DateTime> dates = new();

        IEnumerable<News> result = news.GetNewsDates(out dates, _timeProvider.Object);

        dates.Should().HaveCount(2);
        dates[0].Month.Should().Be(2);
        dates[0].Year.Should().Be(2016);
        dates[1].Year.Should().Be(2016);
        dates[1].Month.Should().Be(3);

        result.Should().BeEquivalentTo(news);
    }

    [Fact]
    public void ShouldReturnOneDateForDuplicateMonths()
    {
        List<News> news = new()
        {
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty)
        };

        List<DateTime> dates = new();

        IEnumerable<News> result = news.GetNewsDates(out dates, _timeProvider.Object);

        dates.Should().HaveCount(1);
        dates[0].Month.Should().Be(2);
        dates[0].Year.Should().Be(2016);

        result.Should().BeEquivalentTo(news);
    }

    [Fact]
    public void ShouldReturnOnlyReturnCurrentAndPastNewsItems()
    {
        List<News> news = new()
        {
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image caption",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 02, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image caption",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 03, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image caption",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 06, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 10, 01),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty),
            new News("title",
                    "slug",
                    "teaser",
                    "purpose",
                    "image",
                    "hero image caption",
                    "thumbnail",
                    "body",
                    "hero image caption",
                    new DateTime(2016, 12, 10),
                    new DateTime(2016, 12, 25),
                    new DateTime(2016, 12, 25),
                    new DateTime(2016, 12, 25),
                    new DateTime(2016, 05, 01),
                    new List<Crumb>(),
                    new List<Alert>(),
                    new List<string>(),
                    new List<Document>(),
                    new List<string>(),
                    new List<Profile>(),
                    null,
                    null,
                    string.Empty,
                    null,
                    null,
                    string.Empty)
        };

        List<DateTime> dates = new();

        IEnumerable<News> result = news.GetNewsDates(out dates, _timeProvider.Object);

        dates.Should().HaveCount(3);
        dates[0].Month.Should().Be(2);
        dates[0].Year.Should().Be(2016);

        result.Should().BeEquivalentTo(news);
    }
}
