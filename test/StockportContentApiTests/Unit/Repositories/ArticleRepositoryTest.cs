using System;
using System.Collections.Generic;
using System.IO;
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
using StockportContentApiTests.Unit.Fakes;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ArticleRepositoryTest
    {
        //private readonly FakeHttpClient _httpClient = new FakeHttpClient();
        private readonly Mock<IHttpClient> _httpClient;
        private readonly ArticleRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private Mock<IFactory<Article>> _mockArticleBuilder;
        private Mock<IVideoRepository> _videoRepository;
        private Mock<ITimeProvider> _mockTimeProvider;
        private readonly DateTime _sunriseDate = new DateTime(2016, 08, 01);
        private readonly DateTime _sunsetDate = new DateTime(2016, 08, 10);

        public ArticleRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _mockArticleBuilder = new Mock<IFactory<Article>>();
            _videoRepository = new Mock<IVideoRepository>();
            _httpClient = new Mock<IHttpClient>();
            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(string.Empty);
            _mockTimeProvider = new Mock<ITimeProvider>();
            _repository = new ArticleRepository(config, _httpClient.Object, _mockArticleBuilder.Object, _videoRepository.Object, _mockTimeProvider.Object);
        }
        
        [Fact]
        public void GetsArticle()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            _mockArticleBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(EmptyArticle());

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GetsNotFoundForAnArticleThatDoesNotExist()
        {
            _mockArticleBuilder.Setup(
                   o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
               .Returns((Article)null);

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=blah"))
                .ReturnsAsync(HttpResponse.Successful(
                    File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("blah"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No article found for 'blah'");
        }

        [Fact]
        public void Gets404ForNewsOutsideOfSunriseDate()
        {
            _mockArticleBuilder.Setup(
                   o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
               .Returns(EmptyArticle());

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 12, 01));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
                 .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));
            //var test = _repository.GetArticle("unit-test-article");
            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Gets404ForNewsOutsideOfSunsetDate()
        {
            _mockArticleBuilder.Setup(
                   o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
               .Returns(EmptyArticle());

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2019, 12, 01));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
                 .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));

            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ReturnsValidSunsetAndSunriseDateWhenDateInRange()
        {
            _mockArticleBuilder.Setup(
                   o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
               .Returns(EmptyArticle());

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
                 .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));

            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private static Article EmptyArticle()
        {
            return new Article("", "", "", "", "", "", "", new List<Section>(), new List<Crumb>(),
                new List<Alert>(), new List<Profile>(), new NullTopic(), new List<Document>(),
                new DateTime(2016, 10, 1), new DateTime(2016, 10, 31), false, new NullLiveChat());
        }
    }
}