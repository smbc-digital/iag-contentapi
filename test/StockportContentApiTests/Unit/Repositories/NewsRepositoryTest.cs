using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using File = System.IO.File;
using HttpResponse = StockportContentApi.Http.HttpResponse;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class NewsRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly NewsRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private const string MockContentfulContentTypesApiUrl = "https://fake.url/spaces/SPACE/content_types?access_token=KEY";
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

        private readonly ContentfulConfig _config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

        private Mock<IBuildContentTypesFromReferences<Alert>> _mockAlertlistFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
        private Mock<IBuildContentTypesFromReferences<Document>> _mockDocumentListFactory = new Mock<IBuildContentTypesFromReferences<Document>>();
        private Mock<IFactory<Newsroom>> _newsroomFactory = new Mock<IFactory<Newsroom>>();
        private Mock<INewsCategoriesFactory> _newsCategoriesFactory = new Mock<INewsCategoriesFactory>();
        private readonly Mock<IContentfulClient> _client;
        private readonly Mock<IContentfulClientManager> _contentfulManager;

        public NewsRepositoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            _httpClient = new Mock<IHttpClient>();
            _videoRepository = new Mock<IVideoRepository>();
            var newsFactory = new Mock<IFactory<News>>();
            var newsroomFactory = new Mock<IFactory<Newsroom>>();

            _newsCategoriesFactory.Setup(o => o.Build(It.IsAny<List<dynamic>>())).Returns(new List<String>()
                 { "Benefits",
                  "Business",
                  "Council leader",
                 });
            _contentfulManager = new Mock<IContentfulClientManager>();
            _client = new Mock<Contentful.Core.IContentfulClient>();
            _contentfulManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);

            _repository = new NewsRepository(_config, _httpClient.Object, newsFactory.Object, newsroomFactory.Object, _newsCategoriesFactory.Object, _mockTimeProvider.Object, _videoRepository.Object, _contentfulManager.Object);

            newsFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(
                new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, 
                new List<string>{ "Bramall Hall" }, new List<Document>(), _newsCategories));
      
            newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));
        }

        [Fact]
        public void GetsANewsItemFromASlug()
        {
            const string slug = "news-of-the-century";
            var document = new Asset
            {
                Description = "metroshuttle route map",
                File = new Contentful.Core.Models.File
                {
                    Url = "document.pdf",
                    FileName = "Stockport-Metroshuttle.pdf",
                    Details = new FileDetails { Size = 674192 }
                },
                SystemProperties = new SystemProperties { UpdatedAt = new DateTime(2016, 10, 5, 11, 09, 48, DateTimeKind.Utc) }
            };

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                 It.Is<QueryBuilder>(q => q.Build() ==
                 new QueryBuilder().ContentTypeIs("news").FieldEquals("fields.slug", slug).Include(1).Build()),
                 It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulNews> { new ContentfulNews
            {
                Title = Title, Alerts = _alerts, Body = Body, Slug = Slug, Teaser = Teaser, SunriseDate =  _sunriseDate,
                SunsetDate =  _sunsetDate, Breadcrumbs = _crumbs, Tags = new List<string>() { "Bramall Hall"},
                Image = new Asset { File = new Contentful.Core.Models.File { Url = Image }  }, Categories = new List<string> { "cat1" },
                Documents = new List<Asset> { document }
            }});

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(Body);

            var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = response.Get<News>();

            news.Title.Should().Be(Title);
            news.Body.Should().Be(Body);
            news.Slug.Should().Be(Slug);
            news.Teaser.Should().Be(Teaser);
            news.SunriseDate.Should().Be(_sunriseDate);
            news.SunsetDate.Should().Be(_sunsetDate);
            news.Image.Should().Be(Image);
            news.ThumbnailImage.Should().Be($"{Image}?h=250");
            news.Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            news.Alerts.Should().BeEquivalentTo(_alerts);
            news.Tags.First().Should().Be("Bramall Hall");
            news.Categories.Count.Should().Be(1);
            news.Categories.First().Should().Be("cat1");
            news.Documents.Count.Should().Be(1);
            news.Documents.First().ShouldBeEquivalentTo(new Document("metroshuttle route map", 674192, new DateTime(2016, 10, 5, 11, 09, 48, DateTimeKind.Utc), "document.pdf", "Stockport-Metroshuttle.pdf"));
        }

        [Fact]
        public void ShouldReturnNotFoundIfNoNewsForSlugFound()
        {
            const string slug = "news-of-the-century";

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                 It.Is<QueryBuilder>(q => q.Build() ==
                 new QueryBuilder().ContentTypeIs("news").FieldEquals("fields.slug", slug).Include(1).Build()),
                 It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulNews>());

            var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public void GetsAllNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));
             _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));
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
            newsroom.Categories.Count.Should().Be(3);
            newsroom.Categories.First().Should().Be("Benefits");
            newsroom.Categories.Last().Should().Be("Council leader");
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

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

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
        public void ShouldReturnListOfNewsForTag()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.tags[in]=Events&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

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

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

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

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&fields.tags[in]=Events&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

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
            var newsfactory = new NewsFactory(_mockAlertlistFactory.Object, _mockDocumentListFactory.Object); 
            var repository = new NewsRepository(_config, _httpClient.Object, newsfactory, _newsroomFactory.Object,_newsCategoriesFactory.Object, _mockTimeProvider.Object, _videoRepository.Object, _contentfulManager.Object);

            _newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));
            
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListingDateTest.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

            var response = AsyncTestHelper.Resolve(repository.Get(tag: null, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            newsroom.News.Count.Should().Be(1);
            newsroom.News.First().Title.Should().Be("This is within the date Range");
        }

        [Fact]
        public void ShouldReturnListOfFilterDatesForAllNewsThatIsCurrentOrPast()
        {
            var newsfactory = new NewsFactory(_mockAlertlistFactory.Object, _mockDocumentListFactory.Object);
            var repository = new NewsRepository(_config, _httpClient.Object, newsfactory, _newsroomFactory.Object,  _newsCategoriesFactory.Object,_mockTimeProvider.Object, _videoRepository.Object, _contentfulManager.Object);

            _newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 7));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListingDateTest.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

            var response = AsyncTestHelper.Resolve(repository.Get(tag: null, category: null, startDate: null, endDate: null));
            var newsroom = response.Get<Newsroom>();

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

        [Fact]
        public void ShouldReturnNewsItemsWithExactMatchingesForTagsWithoutHash()
        {
            var newsfactory = new NewsFactory(_mockAlertlistFactory.Object, _mockDocumentListFactory.Object);
            var repository = new NewsRepository(_config, _httpClient.Object, newsfactory, _newsroomFactory.Object, _newsCategoriesFactory.Object, _mockTimeProvider.Object, _videoRepository.Object, _contentfulManager.Object);
            
            _newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 5));

            var tag = "testTag";

            var queryString = $"&content_type=news&include=1&fields.tags[in]={tag}&limit={ContentfulQueryValues.LIMIT_MAX}";

            _httpClient.Setup(client => client.Get($"{MockContentfulApiUrl}{queryString}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListingWithoutHashTagTest.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));
            // Act
            var response = AsyncTestHelper.Resolve(repository.Get(tag: tag, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            // Assert
            _httpClient.Verify(client => client.Get($"{MockContentfulApiUrl}{queryString}"), Times.Once());
            newsroom.News.Count.Should().Be(1);
            newsroom.News.First().Tags.Any(t => t == tag).Should().BeTrue(); ;
        }

        [Fact]
        public void ShouldReturnNewsItemsWithTagsContainingMatchingTagsWithHash()
        {            
            var newsfactory = new NewsFactory(_mockAlertlistFactory.Object, _mockDocumentListFactory.Object);
            var repository = new NewsRepository(_config, _httpClient.Object, newsfactory, _newsroomFactory.Object, _newsCategoriesFactory.Object, _mockTimeProvider.Object, _videoRepository.Object, _contentfulManager.Object);

            _newsroomFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(new Newsroom(_alerts, true, "test-id"));

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 5));

            const string tag = "#testTag";
            const string expectedTagQueryValue = "testTag";

            var queryString = $"&content_type=news&include=1&fields.tags[match]={expectedTagQueryValue}&limit={ContentfulQueryValues.LIMIT_MAX}";
            
            _httpClient.Setup(client => client.Get($"{MockContentfulApiUrl}{queryString}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListingWithHashTagTest.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));
            // Act
            var response = AsyncTestHelper.Resolve(repository.Get(tag: tag, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            // Assert
            _httpClient.Verify(client => client.Get($"{MockContentfulApiUrl}{queryString}"), Times.Once());
            newsroom.News.Count.Should().Be(2);            

            newsroom.News[1].Tags.Any(t => t == tag).Should().BeTrue();
        }

        [Fact]
        public void GetsTheTopTwoNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1&limit={ContentfulQueryValues.LIMIT_MAX}"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulContentTypesApiUrl}"))
            .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentTypes.json")));

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
    }
}
