using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using HttpResponse = StockportContentApi.Http.HttpResponse;

namespace StockportContentApiTests.Unit.Repositories
{
    public class NewsRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly NewsRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IVideoRepository> _videoRepository;
        private const string Title = "This is the news";
        private const string Body = "The news";
        private const string Slug = "news-of-the-century";
        private const string Teaser = "Read more for the news";
        private readonly DateTime _sunriseDate = new DateTime(2016, 08, 01);
        private readonly DateTime _sunsetDate = new DateTime(2016, 08, 10);
        private const string Image = "image.jpg";
        private const string ThumbnailImage = "thumbnail.jpg";
        private readonly List<Crumb> _crumbs = new List<Crumb>() { new Crumb("title", "slug", "type") };
        private readonly List<Alert> _alerts = new List<Alert>() {
                new Alert("title", "subheading", "body", "severity", new DateTime(2016, 08, 5), new DateTime(2016, 08, 11)) };
        private readonly List<string> _newsCategories = new List<string>() { "news-category-1", "news-category-2" };

        public NewsRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _mockTimeProvider = new Mock<ITimeProvider>();
            _httpClient = new Mock<IHttpClient>();
            _videoRepository = new Mock<IVideoRepository>();
            var newsFactory = new Mock<IFactory<News>>();
            var newsroomFactory = new Mock<IFactory<Newsroom>>();
            _repository = new NewsRepository(config, _httpClient.Object, newsFactory.Object, newsroomFactory.Object, _mockTimeProvider.Object, _videoRepository.Object);

            newsFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(
                new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, 
                new List<string>{ "Bramall Hall" }, new List<Document>(), _newsCategories));
            newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));
        }

        [Fact]
        public void GetsAllNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null,null,null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var newsroom = response.Get<Newsroom>();

            newsroom.Alerts.Count.Should().Be(1);
            newsroom.Alerts.Should().BeEquivalentTo(_alerts);

            newsroom.EmailAlerts.Should().Be(true);
            newsroom.EmailAlertsTopicId.Should().Be("test-id");
            newsroom.Categories.Count.Should().Be(2);
            newsroom.Categories[0].Should().Be("news-category-1");
            newsroom.Categories[1].Should().Be("news-category-2");
            newsroom.Dates.Should().HaveCount(1);
            newsroom.Dates[0].Should().HaveMonth(8);
            newsroom.Dates[0].Should().HaveYear(2016);


            newsroom.News.Count.Should().Be(2);
            var firstNews = newsroom.News.First();
            firstNews.Title.Should().Be(Title);
            firstNews.Body.Should().Be(Body);
            firstNews.Slug.Should().Be(Slug);
            firstNews.Teaser.Should().Be(Teaser);
            firstNews.SunriseDate.Should().Be(_sunriseDate);
            firstNews.SunsetDate.Should().Be(_sunsetDate);
            firstNews.Image.Should().Be(Image);
            firstNews.ThumbnailImage.Should().Be(ThumbnailImage);
            firstNews.Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            firstNews.Alerts.Should().BeEquivalentTo(_alerts);
            firstNews.Categories.Count.Should().Be(2);
            firstNews.Categories[0].Should().Be("news-category-1");
            firstNews.Categories[1].Should().Be("news-category-2");
        }

        [Fact]
        public void GetsAllNewsItemsWhenNoNewsroomIsPresent()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var newsroom = response.Get<Newsroom>();

            newsroom.Alerts.Count.Should().Be(0);
            newsroom.News.Count.Should().Be(2);
            newsroom.News.First().Title.Should().Be(Title);
            newsroom.News.First().Body.Should().Be(Body);
            newsroom.News.First().Slug.Should().Be(Slug);
            newsroom.News.First().Teaser.Should().Be(Teaser);
            newsroom.News.First().SunriseDate.Should().Be(_sunriseDate);
            newsroom.News.First().SunsetDate.Should().Be(_sunsetDate);
            newsroom.News.First().Image.Should().Be(Image);
            newsroom.News.First().ThumbnailImage.Should().Be(ThumbnailImage);
            newsroom.News.First().Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            newsroom.News.First().Alerts.Should().BeEquivalentTo(_alerts);
            newsroom.Dates.Should().HaveCount(1);
            newsroom.Dates[0].Should().HaveMonth(8);
            newsroom.Dates[0].Should().HaveYear(2016);
        }

        [Fact]
        public void GetsASingleNewsItemFromASlug()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.slug=news-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/News.json")));

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");

            var response = AsyncTestHelper.Resolve(_repository.GetNews("news-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = response.Get<News>();

            news.Title.Should().Be(Title);
            news.Body.Should().Be(Body);
            news.Slug.Should().Be(Slug);
            news.Teaser.Should().Be(Teaser);
            news.SunriseDate.Should().Be(_sunriseDate);
            news.SunsetDate.Should().Be(_sunsetDate);
            news.Image.Should().Be(Image);
            news.ThumbnailImage.Should().Be(ThumbnailImage);
            news.Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            news.Alerts.Should().BeEquivalentTo(_alerts);
            news.Tags.First().Should().Be("Bramall Hall");
        }

        [Fact]
        public void Gets404ForNewsOutsideOfSunriseDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 06, 01));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.slug=news-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/News.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetNews("news-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Gets404ForNewsOutsideOfSunsetDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 10, 01));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.slug=news-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/News.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetNews("news-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void GetsTheTopTwoNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");

            var response = AsyncTestHelper.Resolve(_repository.GetNewsByLimit(2));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = response.Get<List<News>>();

            news.Count.Should().Be(2);
            news.First().Title.Should().Be(Title);
            news.First().Body.Should().Be(Body);
            news.First().Slug.Should().Be(Slug);
            news.First().Teaser.Should().Be(Teaser);
            news.First().SunriseDate.Should().Be(_sunriseDate);
            news.First().SunsetDate.Should().Be(_sunsetDate);
            news.First().Image.Should().Be(Image);
            news.First().ThumbnailImage.Should().Be(ThumbnailImage);
            news.First().Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            news.First().Alerts.Should().BeEquivalentTo(_alerts);
        }

        [Fact]
        public void ShouldReturnListOfNewsForTag()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.tags[in]=Events"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(tag: "Events", category: null, startDate:null, endDate: null));
            var newsroom = response.Get<Newsroom>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            newsroom.Alerts.Count.Should().Be(1);

            newsroom.EmailAlerts.Should().Be(true);
            newsroom.EmailAlertsTopicId.Should().Be("test-id");

            newsroom.News.Count.Should().Be(2);
            newsroom.News.First().Title.Should().Be(Title);
            newsroom.News.First().Slug.Should().Be(Slug);
        }

        [Fact]
        public void ShouldReturnListOfNewsForCategory()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: "news-category-1",startDate:null, endDate: null));
            var newsroom = response.Get<Newsroom>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            newsroom.Alerts.Count.Should().Be(1);

            newsroom.EmailAlerts.Should().Be(true);
            newsroom.EmailAlertsTopicId.Should().Be("test-id");

            newsroom.News.Count.Should().Be(2);
            newsroom.News.First().Title.Should().Be(Title);
            newsroom.News.First().Slug.Should().Be(Slug);
        }

        [Fact]
        public void ShouldReturnListOfNewsForCategoryAndTag()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.tags[in]=Events"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(tag: "Events", category: "news-category-1", startDate: null, endDate: null));
            var newsroom = response.Get<Newsroom>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            newsroom.Alerts.Count.Should().Be(1);

            newsroom.EmailAlerts.Should().Be(true);
            newsroom.EmailAlertsTopicId.Should().Be("test-id");

            newsroom.News.Count.Should().Be(2);
            newsroom.News.First().Title.Should().Be(Title);
            newsroom.News.First().Slug.Should().Be(Slug);
        }

        [Fact]
        public void ShouldReturnListOfNewsForDateRange()
        {
            // Arrange
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")  
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();
          
            var mockAlertlistFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            var mockDocumentListFactory = new Mock<IBuildContentTypesFromReferences<Document>>();           
            var newsroomFactory = new Mock<IFactory<Newsroom>>();
            var newsfactory = new NewsFactory(mockAlertlistFactory.Object, mockDocumentListFactory.Object); 
            var repository = new NewsRepository(config, _httpClient.Object, newsfactory, newsroomFactory.Object, _mockTimeProvider.Object, _videoRepository.Object);
            newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));
            
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListingDateTest.json")));
            
            // Act
            var response = AsyncTestHelper.Resolve(repository.Get(tag: null, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            // Assert
            newsroom.News.Count.Should().Be(1);
            newsroom.News.First().Title.Should().Be("This is within the date Range");
        }

        [Fact]
        public void ShouldReturnListOfFilterDatesForAllNewsThatIsCurrentOrPast()
        {
            // Arrange
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            var mockAlertlistFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            var mockDocumentListFactory = new Mock<IBuildContentTypesFromReferences<Document>>();
            var newsroomFactory = new Mock<IFactory<Newsroom>>();
            var newsfactory = new NewsFactory(mockAlertlistFactory.Object, mockDocumentListFactory.Object);
            var repository = new NewsRepository(config, _httpClient.Object, newsfactory, newsroomFactory.Object, _mockTimeProvider.Object, _videoRepository.Object);
            newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListingDateTest.json")));

            // Act
            var response = AsyncTestHelper.Resolve(repository.Get(tag: null, category: null, startDate: null, endDate: null));
            var newsroom = response.Get<Newsroom>();

            // Assert
            newsroom.Dates.Count.Should().Be(2);
            newsroom.Dates.First().Date.Should().Be(new DateTime(2016, 08, 01));
        }


        [Fact]
        public void ShouldReturnNotFoundForTagAndCategory()
        {
            var response = AsyncTestHelper.Resolve(_repository.Get("NotFound", "NotFound",null,null));
            var newsroom = response.Get<Newsroom>();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            newsroom.Should().BeNull();
        }
    }
}
