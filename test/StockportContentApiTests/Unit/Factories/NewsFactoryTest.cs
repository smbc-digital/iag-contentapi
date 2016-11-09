using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class NewsFactoryTest
    {
        private readonly IFactory<News> _newsFactory;
        private const string ThumbnailImageExtension = "?h=250";
        private const string AlertTitle = "Title";
        private const string AlertSubHeading = "SubHeading";
        private const string AlertBody = "Body";
        private const string AlertSeverity = "Error";
        private readonly DateTime _alertSunriseDate = new DateTime(2016, 10, 10);
        private readonly DateTime _alertSunsetDate = new DateTime(2016, 10, 20);
        private readonly List<Document> _documents = new List<Document>() { new Document("Title", 1212, DateTime.Now, "/thisisaurl", "filename1.pdf"),
                                                                            new Document("Title 2", 3412, DateTime.Now.AddHours(2), "/anotherurl", "filename2.pdf") };

        public NewsFactoryTest()
        {
            var mockAlertListFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            var mockDocumentListFactory = new Mock<IBuildContentTypesFromReferences<Document>>();

            var alert = new Alert(AlertTitle, AlertSubHeading, AlertBody, AlertSeverity, _alertSunriseDate, _alertSunsetDate);
           mockAlertListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Alert>() { alert });
            mockDocumentListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>())).Returns(_documents);
               
            _newsFactory = new NewsFactory(mockAlertListFactory.Object,mockDocumentListFactory.Object);
        }

        [Fact]
        public void BuildsNews()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/News.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            News news = _newsFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            news.Title.Should().Be("This is the news");
            news.Slug.Should().Be("news-of-the-century");
            news.Teaser.Should().Be("Read more for the news");
            news.Image.Should().Be("image.jpg");
            news.ThumbnailImage.Should().Be("image.jpg" + ThumbnailImageExtension);
            news.Body.Should().Be("The news {{PDF:Stockport-Metroshuttle.pdf}} {{PDF:a-pdf.pdf}}");
            news.SunriseDate.Should().Be(DateTime.Parse("2016-07-10T00:00:00+01:00"));
            news.SunsetDate.Should().Be(DateTime.Parse("2016-08-24T00:00:00+01:00"));

            news.Breadcrumbs.Should().HaveCount(1);
            news.Breadcrumbs.First().Title.Should().Be("News");
            news.Breadcrumbs.First().Slug.Should().BeEmpty();
            news.Breadcrumbs.First().Type.Should().Be("news");
            news.Alerts.Count.Should().Be(1);

            var alert = news.Alerts.First();
            alert.Title.Should().Be(AlertTitle);
            alert.SubHeading.Should().Be(AlertSubHeading);
            alert.Body.Should().Be(AlertBody);
            alert.Severity.Should().Be(AlertSeverity);
            alert.SunriseDate.Should().Be(_alertSunriseDate);
            alert.SunsetDate.Should().Be(_alertSunsetDate);

            news.Tags.First().Should().Be("Bramall Hall");

            var document = news.Documents.First();
            document.Title.Should().Be(_documents.First().Title);
            document.FileName.Should().Be(_documents.First().FileName);
            document.Size.Should().Be(_documents.First().Size);
            document.Url.Should().Be(_documents.First().Url);

            news.Categories.Count.Should().Be(2);
            news.Categories[0].Should().Be("Category 1");
            news.Categories[1].Should().Be("Category 2");
        }

        [Fact]
        public void BuildsNewsWithoutBackgrounds()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    File.ReadAllText("Unit/MockContentfulResponses/NewsWithoutBackgrounds.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            News news = _newsFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            news.Title.Should().Be("This is the news");
            news.Slug.Should().Be("news-of-the-century");
            news.Teaser.Should().Be("The news");
            news.Image.Should().Be("");
            news.ThumbnailImage.Should().Be("");
            news.Body.Should().Be("The news now");
            news.SunriseDate.Should().Be(DateTime.Parse("2016-08-25T00:00+01:00"));
            news.SunsetDate.Should().Be(DateTime.Parse("2016-08-30T00:00+01:00"));
        }
    }
}
