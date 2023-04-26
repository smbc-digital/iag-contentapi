﻿namespace StockportContentApiTests.Unit.Extensions;

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
        var news = new List<News>()
        {
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 03, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
        };

        var dates = new List<DateTime>();

        var result = news.GetNewsDates(out dates, _timeProvider.Object);

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
        var news = new List<News>
        {
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
        };

        var dates = new List<DateTime>();

        var result = news.GetNewsDates(out dates, _timeProvider.Object);

        dates.Should().HaveCount(1);
        dates[0].Month.Should().Be(2);
        dates[0].Year.Should().Be(2016);

        result.Should().BeEquivalentTo(news);
    }

    [Fact]
    public void ShouldReturnOnlyReturnCurrentAndPastNewsItems()
    {
        var news = new List<News>
        {
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 03, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 06, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
            new News("title", "slug", "teaser", "purpose", "image", "thumbnail", "body", new DateTime(2016, 12, 10), new DateTime(2016, 12, 25), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
        };

        var dates = new List<DateTime>();

        var result = news.GetNewsDates(out dates, _timeProvider.Object);

        dates.Should().HaveCount(3);
        dates[0].Month.Should().Be(2);
        dates[0].Year.Should().Be(2016);

        result.Should().BeEquivalentTo(news);
    }
}
