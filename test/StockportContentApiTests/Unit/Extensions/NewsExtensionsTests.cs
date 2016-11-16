using Xunit;
using FluentAssertions;
using StockportContentApi.Extensions;
using System.Collections.Generic;
using StockportContentApi.Model;
using System;

namespace StockportContentApiTests.Unit.Extensions
{
    public class NewsExtensionsTests
    {
        [Fact]
        public void ShouldReturnTwoDatesForNewsItems()
        {
            var news = new List<News>()
            {
                new News("title", "slug", "teaser", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
                new News("title", "slug", "teaser", "image", "thumbnail", "body", new DateTime(2016, 03, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
            };

            var dates = new List<DateTime>();

            var result = NewsExtensions.GetNewsDates(news, out dates);

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
            var news = new List<News>()
            {
                new News("title", "slug", "teaser", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>()),
                new News("title", "slug", "teaser", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
            };

            var dates = new List<DateTime>();

            var result = NewsExtensions.GetNewsDates(news, out dates);

            dates.Should().HaveCount(1);
            dates[0].Month.Should().Be(2);
            dates[0].Year.Should().Be(2016);

            result.Should().BeEquivalentTo(news);
        }

        [Fact]
        public void ShouldReturnNewsCategories()
        {
            var newsCategories = new List<string>()
            {
                "test",
                "business"
            };

            var news = new List<News>()
            {
                new News("title", "slug", "teaser", "image", "thumbnail", "body", new DateTime(2016, 02, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), newsCategories),
                new News("title", "slug", "teaser", "image", "thumbnail", "body", new DateTime(2016, 03, 01), new DateTime(2016, 10, 01), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
            };

            var categories = new List<string>();

            var result = NewsExtensions.GetTheCategories(news, out categories);

            categories.Should().HaveCount(2);
            categories[0].Should().Be("test");
            categories[1].Should().Be("business");

            result.Should().BeEquivalentTo(news);
        }
    }
}
