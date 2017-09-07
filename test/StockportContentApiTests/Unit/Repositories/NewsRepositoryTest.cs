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
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Extensions;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using File = System.IO.File;
using HttpResponse = StockportContentApi.Http.HttpResponse;
using IContentfulClient = Contentful.Core.IContentfulClient;
using System.Collections;
using Contentful.Core.Models.Management;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StockportContentApiTests.Unit.Repositories
{
    public class NewsRepositoryTest
    {
        private readonly NewsRepository _repository;
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
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulNews, News>> _newsContentfulFactory;
        private readonly Mock<IContentfulFactory<ContentfulNewsRoom, Newsroom>> _newsRoomContentfulFactory;
        private readonly Mock<IContentfulClient> _client;
        private readonly Mock<IContentfulClientManager> _contentfulManager;
        private readonly NewsContentfulFactory _contentfulFactory;
        private readonly Mock<IContentfulClientManager> _contentfulClientManager;
        private readonly ContentType _newsContentType;
        private readonly ContentfulCollection<ContentfulNewsRoom> _newsroomContentfulCollection;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<IConfiguration> _configuration;

        public NewsRepositoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            _videoRepository = new Mock<IVideoRepository>();
            _cacheWrapper = new Mock<ICache>();

            _newsContentType = new ContentType()
            {
                Fields = new List<Field>()
                {
                    new Field()
                    {
                        Name = "Categories",
                        Items = new Contentful.Core.Models.Schema()
                        {
                            Validations = new List<IFieldValidator>()
                            {
                                new InValuesValidator {RequiredValues = new List<string>() { "Benefits","Business","Council leader" } }
                            }
                        }
                    }
                }
            };
            
            _newsroomContentfulCollection = new ContentfulCollection<ContentfulNewsRoom>();
            _newsroomContentfulCollection.Items = new List<ContentfulNewsRoom>
            {
                new ContentfulNewsRoomBuilder().Build()
            };
            _contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<Contentful.Core.IContentfulClient>();
            _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
            _contentfulFactory = new NewsContentfulFactory(_videoRepository.Object, new DocumentContentfulFactory());

            _client.Setup(o => o.GetEntriesAsync(
            It.Is<QueryBuilder<ContentfulNewsRoom>>(q => q.Build() == new QueryBuilder<ContentfulNewsRoom>().ContentTypeIs("newsroom").Include(1).Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(_newsroomContentfulCollection);

            _client.Setup(o => o.GetContentTypeAsync("news", It.IsAny<CancellationToken>()))
               .ReturnsAsync(_newsContentType);

            _newsContentfulFactory = new Mock<IContentfulFactory<ContentfulNews, News>>();
            _newsRoomContentfulFactory = new Mock<IContentfulFactory<ContentfulNewsRoom, Newsroom>>();
           
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:News"]).Returns("60");
            _repository = new NewsRepository(_config, _mockTimeProvider.Object, _contentfulClientManager.Object, _newsContentfulFactory.Object, _newsRoomContentfulFactory.Object, _cacheWrapper.Object, _configuration.Object);
        }
        
        [Fact]
        public void GetsANewsItemFromASlug()
        {
            // Arrange
            const string slug = "news-of-the-century";
            List<Alert> alerts = new List<Alert> { new Alert("New alert", "alert sub heading updated", "Alert body",
                                                                 "Error", new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc),
                                                                  new DateTime(2017, 11, 22, 22, 0, 0, DateTimeKind.Utc)) };
            _mockTimeProvider.Setup(o => o.Now()).Returns(DateTime.Now);
           
            var contentfulNews = new ContentfulNewsBuilder().Title("This is the news").Body("The news").Teaser("Read more for the news").Slug(slug).SunriseDate(new DateTime(2016, 08, 01)).SunsetDate(new DateTime(2016, 08, 10)).Build();
            var collection = new ContentfulCollection<ContentfulNews>();
            collection.Items = new List<ContentfulNews> { contentfulNews };
            var newsCollection = new List<ContentfulNews> { contentfulNews };
            var simpleNewsQuery =
                new QueryBuilder<ContentfulNews>()
                    .ContentTypeIs("news")
                    .FieldEquals("fields.slug", slug)
                    .Include(1)
                    .Build();
            _client.Setup(o => o.GetEntriesAsync(
                    It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == simpleNewsQuery),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);
            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(contentfulNews.Body);
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsCollection);


            var newsItem = new News(Title, Slug, Teaser, Image, ImageConverter.ConvertToThumbnail(Image), Body, _sunriseDate, _sunsetDate, _crumbs, alerts, null, new List<Document>(), new List<string> { "A category" });

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(newsItem);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = response.Get<News>();        
            news.ShouldBeEquivalentTo(contentfulNews, o => o.Excluding(e => e.Image).Excluding(e => e.ThumbnailImage).Excluding(e => e.Documents).Excluding(e => e.Breadcrumbs).Excluding(e=>e.Tags));
            news.Image.Should().Be(contentfulNews.Image.File.Url);
            news.ThumbnailImage.Should().Be(contentfulNews.Image.File.Url + "?h=250");
        }

        [Fact]
        public void ShouldReturnNotFoundIfNoNewsForSlugFound()
        {
            const string slug = "news-of-the-century";
            var collection = new ContentfulCollection<ContentfulNews>();
            collection.Items = new List<ContentfulNews>();

            _mockTimeProvider.Setup(o => o.Now()).Returns(DateTime.Now);
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(collection.Items.ToList());

            var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));



            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public void GetsAllNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            var contentfulNewsRoom = new ContentfulNewsRoom { Title = "test" };
            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { "tag1", "tag2" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);          
                
            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(contentfulNewsRoom);
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-categories"), It.IsAny<Func<Task<List<string>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<string> { "Benefits", "foo", "Council leader" });

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null,null,null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var newsroom = response.Get<Newsroom>();

            newsroom.Alerts.Count.Should().Be(1);
            newsroom.Alerts[0].Title.Should().BeEquivalentTo(_alerts[0].Title);
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

            var newsRoom = new Newsroom(new List<Alert> { }, true, "");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, null, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });


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

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { "Events" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).FieldEquals("fields.tags[in]", "Events").Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

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

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { "Events" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

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

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { "Events" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).FieldEquals("fields.tags[in]", "Events").Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

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
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News("This is within the date Range", Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, null, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("This is within the date Range").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is within the date Range").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

            var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            newsroom.News.Count.Should().Be(2);
            newsroom.News.First().Title.Should().Be("This is within the date Range");
        }

        [Fact]
        public void ShouldReturnNoListOfNewsForDateRange()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News("This is within the date Range", Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, null, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("This is within the date Range").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is within the date Range").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });


            var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: null, startDate: new DateTime(2017, 08, 01), endDate: new DateTime(2017, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            newsroom.News.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldReturnListOfFilterDatesForAllNewsThatIsCurrentOrPast()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { "tag1", "tag2" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });



            var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: null, startDate: null, endDate: null));
            var newsroom = response.Get<Newsroom>();

            newsroom.Dates.Count.Should().Be(1);
            newsroom.Dates.First().Date.Should().Be(new DateTime(2016, 08, 01));
        }


        [Fact]
        public void ShouldReturnNotFoundForTagAndCategory()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() {"tag1", "tag2" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

            var response = AsyncTestHelper.Resolve(_repository.Get("NotFound", "NotFound", null,null));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ShouldReturnNewsItemsWithExactMatchingesForTagsWithoutHash()
        {
            const string tag = "testTag";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { "testTag" }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Tags(new List<string> { "testTag", "foo" }).Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Tags(new List<string> { "testTag", "bar" }).Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.Get(tag: tag, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
            var newsroom = response.Get<Newsroom>();

            // Assert
            newsroom.News.Count.Should().Be(2);
            newsroom.News.First().Tags.Any(t => t == tag).Should().BeTrue(); ;
        }

        [Fact]
        public void ShouldReturnNewsItemsWithTagsContainingMatchingTagsWithHash()
        {
            const string tag = "#testTag";
            const string expectedTagQueryValue = "testTag";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, new List<string>() { expectedTagQueryValue }, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Tags(new List<string> { "#testTag", "foo" }).Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Tags(new List<string> { "#testTag", "foo" }).Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            };
            _client.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).FieldEquals("fields.tags[match]", "testTag").Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.Get(tag, null, null, null));
            var newsroom = response.Get<Newsroom>();

            // Assert
            newsroom.News.Count.Should().Be(2);
            newsroom.News[1].Tags.Any(t => t == expectedTagQueryValue).Should().BeTrue();
        }

        [Fact]
        public void GetsTheTopTwoNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            var newsRoom = new Newsroom(_alerts, true, "test-id");
            _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

            var news = new News(Title, Slug, Teaser, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _crumbs, _alerts, null, new List<Document>(), _newsCategories);

            _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

            var newsListCollection = new ContentfulCollection<ContentfulNews>();
            newsListCollection.Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder().Title("This is the first news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build()
            };
            _client.Setup(o => o.GetEntriesAsync(
                It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Limit(1000).Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns("The news");
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });


            var response = AsyncTestHelper.Resolve(_repository.GetNewsByLimit(2));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var newsList = response.Get<List<News>>();

            newsList.Count.Should().Be(2);
            newsList.First().Title.Should().Be(Title);
            newsList.First().Body.Should().Be(Body);
            newsList.First().Slug.Should().Be(Slug);
            newsList.First().Teaser.Should().Be(Teaser);
            newsList.First().SunriseDate.Should().Be(_sunriseDate);
            newsList.First().SunsetDate.Should().Be(_sunsetDate);
            newsList.First().Image.Should().Be(Image);
            newsList.First().ThumbnailImage.Should().Be(ThumbnailImage);
            newsList.First().Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            newsList.First().Alerts.Should().BeEquivalentTo(_alerts);
        }

        [Fact]
        public void ShouldReturnNotFoundIfNewsHasSunriseDateAfterToday()
        {
            // Arrange
            const string slug = "news-with-sunrise-date-in-future";
            DateTime nowDateTime = DateTime.Now;
            DateTime futureSunRiseDate = DateTime.Now.AddDays(10);
            _mockTimeProvider.Setup(o => o.Now()).Returns(nowDateTime);
            var newsWithSunriseDateInFuture = new ContentfulNewsBuilder().SunriseDate(futureSunRiseDate).Slug(slug).Build();
            var collection = new ContentfulCollection<ContentfulNews>();
            collection.Items = new List<ContentfulNews> { newsWithSunriseDateInFuture };

            var simpleNewsQuery =
                new QueryBuilder<ContentfulNews>()
                    .ContentTypeIs("news")
                    .FieldEquals("fields.slug", slug)
                    .Include(1)
                    .Build();

            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(newsWithSunriseDateInFuture.Body);

            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(collection.Items.ToList());
            
            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ShouldReturnSuccessIfNewsArticleSunsetDateIsInThePast()
        {
            // Arrange
            const string slug = "news-with-sunrise-date-in-future";
            DateTime nowDateTime = DateTime.Now;
            DateTime pastSunRiseDate = DateTime.Now.AddDays(-20);
            DateTime pastSunSetDate = DateTime.Now.AddDays(-10);
            _mockTimeProvider.Setup(o => o.Now()).Returns(nowDateTime);
            var newsWithSunsetDateInPast = new ContentfulNewsBuilder()
                .SunsetDate(pastSunSetDate)
                .SunriseDate(pastSunRiseDate)
                .Slug(slug)
                .Build();
            var collection = new ContentfulCollection<ContentfulNews>();
            collection.Items = new List<ContentfulNews> { newsWithSunsetDateInPast };
            var simpleNewsQuery =
                new QueryBuilder<ContentfulNews>()
                    .ContentTypeIs("news")
                    .FieldEquals("fields.slug", slug)
                    .Include(1)
                    .Build();
            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(newsWithSunsetDateInPast.Body);
            _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(collection.Items.ToList());

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
