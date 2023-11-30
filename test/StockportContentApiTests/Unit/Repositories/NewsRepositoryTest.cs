using Microsoft.AspNetCore.WebUtilities;

namespace StockportContentApiTests.Unit.Repositories;

public class NewsRepositoryTest
{
    private readonly NewsRepository _repository;
    private readonly Mock<ITimeProvider> _mockTimeProvider;
    private readonly Mock<IVideoRepository> _videoRepository;
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
    private const string Title = "This is the news";
    private const string Body = "The news";
    private const string Slug = "news-of-the-century";
    private const string Teaser = "Read more for the news";
    private const string Purpose = "Purpose";
    private readonly DateTime _sunriseDate = new(2016, 08, 01);
    private readonly DateTime _sunsetDate = new(2016, 08, 10);
    private readonly DateTime _updatedAt = new(2016, 08, 05);
    private const string Image = "image.jpg";
    private const string ThumbnailImage = "thumbnail.jpg";
    private readonly List<Crumb> _crumbs = new() { new("title", "slug", "type") };
    private readonly List<Alert> _alerts = new() {
            new("title", "subheading", "body", "severity", new(2016, 08, 5), new(2016, 08, 11), string.Empty, false, string.Empty) };
    private readonly List<string> _newsCategories = new() { "news-category-1", "news-category-2" };

    private readonly ContentfulConfig _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();

    private readonly Mock<IContentfulFactory<ContentfulNews, News>> _newsContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulNewsRoom, Newsroom>> _newsRoomContentfulFactory;
    private readonly Mock<IContentfulClient> _client;
    private readonly NewsContentfulFactory _contentfulFactory;
    private readonly Mock<IContentfulClientManager> _contentfulClientManager;
    private readonly ContentType _newsContentType;
    private readonly ContentfulCollection<ContentfulNewsRoom> _newsroomContentfulCollection;
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertBuilder;

    public NewsRepositoryTest()
    {
        _mockTimeProvider = new Mock<ITimeProvider>();
        _videoRepository = new Mock<IVideoRepository>();
        _cacheWrapper = new Mock<ICache>();

        _newsContentType = new ContentType() {
            Fields = new() {
                new() {
                    Name = "Categories",
                    Items = new() {
                        Validations = new() {
                            new InValuesValidator {RequiredValues = new() { "Benefits","Business","Council leader" } }
                        }
                    }
                }
            }
        };

        _newsroomContentfulCollection = new ContentfulCollection<ContentfulNewsRoom>
        {
            Items = new List<ContentfulNewsRoom>
            {
                new ContentfulNewsRoomBuilder().Build()
            }
        };

        _contentfulClientManager = new Mock<IContentfulClientManager>();
        _client = new Mock<IContentfulClient>();
        _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
        _alertBuilder = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

        _contentfulFactory = new NewsContentfulFactory(_videoRepository.Object, new DocumentContentfulFactory(), _alertBuilder.Object, _mockTimeProvider.Object, _profileFactory.Object);

        _client.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulNewsRoom>>(q => q.Build() == new QueryBuilder<ContentfulNewsRoom>().ContentTypeIs("newsroom").Include(1).Build()),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_newsroomContentfulCollection);

        _client.Setup(_ => _.GetContentType("news", It.IsAny<CancellationToken>()))
                .ReturnsAsync(_newsContentType);

        _newsContentfulFactory = new Mock<IContentfulFactory<ContentfulNews, News>>();
        _newsRoomContentfulFactory = new Mock<IContentfulFactory<ContentfulNewsRoom, Newsroom>>();

        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(_ => _["redisExpiryTimes:News"]).Returns("60");
        _repository = new NewsRepository(_config, _mockTimeProvider.Object, _contentfulClientManager.Object, _newsContentfulFactory.Object, _newsRoomContentfulFactory.Object, _cacheWrapper.Object, _configuration.Object);
    }

    [Fact]
    public void GetsNews_ShouldReturnItemFromASlug()
    {
        // Arrange
        const string slug = "news-of-the-century";
        List<Alert> alerts = new() { new AlertBuilder().Build() };
        _mockTimeProvider.Setup(_ => _.Now()).Returns(DateTime.Now);

        var contentfulNews = new ContentfulNewsBuilder().Title("This is the news").Body("The news").Teaser("Read more for the news").Slug(slug).SunriseDate(new DateTime(2016, 08, 01)).SunsetDate(new DateTime(2016, 08, 10)).Build();
        ContentfulCollection<ContentfulNews> collection = new() {
            Items = new List<ContentfulNews> { contentfulNews }
        };

        List<ContentfulNews> newsCollection = new() { contentfulNews };

        var simpleNewsQuery =  new QueryBuilder<ContentfulNews>()
            .ContentTypeIs("news")
            .FieldEquals("fields.slug", slug)
            .Include(1)
            .Build();

        _client.Setup(_ => _.GetEntries(
                    It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == simpleNewsQuery),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);
        
        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns(contentfulNews.Body);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsCollection);

        var newsItem = new News(Title, Slug, Teaser, Purpose, Image, ImageConverter.ConvertToThumbnail(Image), Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, alerts, null, new List<Document>(), new List<string> { "A category" }, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(newsItem);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newsItem, response.Get<News>());
    }

    [Fact]
    public void GetNews_ShouldReturnNotFound_If_NewsWithSlugNotFound()
    {
        // Arrange
        var collection = new ContentfulCollection<ContentfulNews> {
            Items = new List<ContentfulNews>()
        };

        _mockTimeProvider.Setup(_ => _.Now()).Returns(DateTime.Now);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(collection.Items.ToList());

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetNews("news-of-the-century"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No news found for 'news-of-the-century'", response.Error);
    }

    [Fact]
    public void GetsAllNewsItems()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));
        ContentfulNewsRoom contentfulNewsRoom = new() { Title = "test" };
        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "tag1", "tag2" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

         ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };

        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(contentfulNewsRoom);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-categories"), It.IsAny<Func<Task<List<string>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<string> { "Benefits", "foo", "Council leader" });

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns("The news");
        
        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var newsroom = response.Get<Newsroom>();
        Assert.Single(newsroom.Alerts);
        Assert.Equal(_alerts[0].Title, newsroom.Alerts[0].Title);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(3, newsroom.Categories.Count);
        Assert.Equal("Benefits", newsroom.Categories.First());
        Assert.Equal("Council leader", newsroom.Categories.Last());
        Assert.Single(newsroom.Dates);
        Assert.Equal(8, newsroom.Dates[0].Month);
        Assert.Equal(2016, newsroom.Dates[0].Year);
        Assert.Equal(2, newsroom.News.Count);

        var firstNews = newsroom.News.First();
        Assert.Equal(Title, firstNews.Title);
        Assert.Equal(Body, firstNews.Body);
        Assert.Equal(Slug, firstNews.Slug);
        Assert.Equal(Teaser, firstNews.Teaser);
        Assert.Equal(_sunriseDate, firstNews.SunriseDate);
        Assert.Equal(_sunsetDate, firstNews.SunsetDate);
        Assert.Equal(_updatedAt, firstNews.UpdatedAt);
        Assert.Equal(Image, firstNews.Image);
        Assert.Equal(ThumbnailImage, firstNews.ThumbnailImage);
        Assert.Equal(_crumbs, firstNews.Breadcrumbs);
        Assert.Equal(_alerts, firstNews.Alerts);
        Assert.Equal(2, firstNews.Categories.Count);
        Assert.Equal("news-category-1", firstNews.Categories[0]);
        Assert.Equal("news-category-2", firstNews.Categories[1]);
    }

    [Fact]
    public void Get_ShouldReturnAllNewsItemsWhenNoNewsroomIsPresent()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));
        Newsroom newsRoom = new(new List<Alert> { }, true, string.Empty);
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        var news = new News(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, null, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns("The news");

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var newsroom = response.Get<Newsroom>();
        Assert.Empty(newsroom.Alerts);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal(Title, newsroom.News.First().Title);
        Assert.Equal(Body, newsroom.News.First().Body);
        Assert.Equal(Slug, newsroom.News.First().Slug);
        Assert.Equal(Teaser, newsroom.News.First().Teaser);
        Assert.Equal(_sunriseDate, newsroom.News.First().SunriseDate);
        Assert.Equal(_sunsetDate, newsroom.News.First().SunsetDate);
        Assert.Equal(_updatedAt, newsroom.News.First().UpdatedAt);
        Assert.Equal(Image, newsroom.News.First().Image);
        Assert.Equal(ThumbnailImage, newsroom.News.First().ThumbnailImage);
        Assert.Equal(_crumbs, newsroom.News.First().Breadcrumbs);
        Assert.Equal(_alerts, newsroom.News.First().Alerts);
        Assert.Single(newsroom.Dates);
        Assert.Equal(8, newsroom.Dates[0].Month);
        Assert.Equal(2016, newsroom.Dates[0].Year);
    }

    [Fact]
    public void ShouldReturnListOfNewsForTag()
    {
        // Arrange
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "Events" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).FieldEquals("fields.tags[in]", "Events").Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: "Events", category: null, startDate: null, endDate: null));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsroom.Alerts);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal(Title, newsroom.News.First().Title);
        Assert.Equal(Slug, newsroom.News.First().Slug);
    }

    [Fact]
    public void ShouldReturnListOfNewsForCategory()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "Events" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: "news-category-1", startDate: null, endDate: null));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsroom.Alerts);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal(Title, newsroom.News.First().Title);
        Assert.Equal(Slug, newsroom.News.First().Slug);
    }

    [Fact]
    public void ShouldReturnListOfNewsForCategoryAndTag()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "Events" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).FieldEquals("fields.tags[in]", "Events").Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: "Events", category: "news-category-1", startDate: null, endDate: null));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsroom.Alerts);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal(Title, newsroom.News.First().Title);
        Assert.Equal(Slug, newsroom.News.First().Slug);
    }

    [Fact]
    public void Get_ShouldReturnListOfNewsForDateRange()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 09, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new("This is within the date range", Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, null, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("This is within the date range").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is within the date range").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal("This is within the date range", newsroom.News.First().Title);
    }

    [Fact]
    public void Get_ShouldReturnNoListOfNewsForDateRange()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 09, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new("This is within the date Range", Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, null, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("This is within the date Range").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is within the date Range").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(o => o.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: null, startDate: new DateTime(2017, 08, 01), endDate: new DateTime(2017, 08, 31)));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Empty(newsroom.News);
    }

    [Fact]
    public void Get_ShouldReturnListOfFilterDatesForAllNewsThatIsCurrentOrPast()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "tag1", "tag2" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: null, category: null, startDate: null, endDate: null));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Single(newsroom.Dates);
        Assert.Equal(new DateTime(2016, 08, 01), newsroom.Dates.First().Date);
    }

    [Fact]
    public void Get_ShouldReturnNotFoundForTagAndCategory()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "tag1", "tag2" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get("NotFound", "NotFound", null, null));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void Get_ShouldReturnNewsItemsWithExactMatchingesForTagsWithoutHash()
    {
        // Arrange
        const string tag = "testTag";
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "testTag" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Tags(new List<string> { "testTag", "foo" }).Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Tags(new List<string> { "testTag", "bar" }).Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag: tag, category: null, startDate: new DateTime(2016, 08, 01), endDate: new DateTime(2016, 08, 31)));
        var newsroom = response.Get<Newsroom>();

        // Assert
        newsroom.News.First().Tags.Any(t => t == tag).Should().BeTrue();
        Assert.Equal(2, newsroom.News.Count);
        Assert.Contains(newsroom.News.First().Tags, t => t.Equals(tag));
    }

    [Fact]
    public void Get_ShouldReturnNewsItemsWithTagsContainingMatchingTagsWithHash()
    {
        // Arrange
        const string tag = "#testTag";
        const string expectedTagQueryValue = "testTag";
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { expectedTagQueryValue }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Tags(new List<string> { "#testTag", "foo" }).Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Tags(new List<string> { "#testTag", "foo" }).Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };
        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).FieldEquals("fields.tags[match]", "testTag").Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(tag, null, null, null));
        var newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(2, newsroom.News.Count);
        Assert.Contains(newsroom.News[1].Tags, t => t.Equals(expectedTagQueryValue));
    }

    [Fact]
    public void GetNewsByLimit_ShouldReturnTopNewsItems()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2020, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);
        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>()))
                .Returns<ContentfulNews>(_ => new News(_.Title, _.Slug, _.Teaser, null, null, null, null, _.SunriseDate, _.SunsetDate, DateTime.MinValue, null, null, null, new List<Document>(), null, null));

        ContentfulCollection<ContentfulNews> newsListCollection = new();
        var earliestNewsItem = new ContentfulNewsBuilder().Title("This is the first news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2025, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build();
        var middleNewsItem = new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2017, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2025, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build();
        var latestNewsItem = new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2018, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2025, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build();
        newsListCollection.Items = new List<ContentfulNews>
        {
            earliestNewsItem,
            middleNewsItem, 
            latestNewsItem
        };

        _client.Setup(_ => _.GetEntries(
                    It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Limit(1000).Build()),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns("The news");
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetNewsByLimit(1));

        // Arrange
        var newsList = response.Get<List<News>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsList);
        Assert.Equal(latestNewsItem.Title, newsList.First().Title);
        Assert.Equal(latestNewsItem.Slug, newsList.First().Slug);
        Assert.Equal(latestNewsItem.Teaser, newsList.First().Teaser);
        Assert.Equal(latestNewsItem.SunriseDate, newsList.First().SunriseDate);
        Assert.Equal(latestNewsItem.SunsetDate, newsList.First().SunsetDate);
    }

    [Fact]
    public void GetNewsByLimit_ShouldReturnTopTwoNewsItems()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, null, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("This is the first news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build()
            }
        };

        _client.Setup(_ => _.GetEntries(
                    It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Limit(1000).Build()),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns("The news");
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetNewsByLimit(2));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var newsList = response.Get<List<News>>();

        Assert.Equal(2, newsList.Count);
        Assert.Equal(Title, newsList.First().Title);
        Assert.Equal(Body, newsList.First().Body);
        Assert.Equal(Slug, newsList.First().Slug);
        Assert.Equal(Teaser, newsList.First().Teaser);
        Assert.Equal(_sunriseDate, newsList.First().SunriseDate);
        Assert.Equal(_sunsetDate, newsList.First().SunsetDate);
        Assert.Equal(_updatedAt, newsList.First().UpdatedAt);
        Assert.Equal(Image, newsList.First().Image);
        Assert.Equal(ThumbnailImage, newsList.First().ThumbnailImage);
        Assert.Equal(_crumbs, newsList.First().Breadcrumbs);
        Assert.Equal(_alerts, newsList.First().Alerts);
    }

    [Fact]
    public void GetNews_ShouldReturnNotFoundIfNewsHasSunriseDateAfterToday()
    {
        // Arrange
        const string slug = "news-with-sunrise-date-in-future";
        DateTime nowDateTime = DateTime.Now;
        DateTime futureSunRiseDate = DateTime.Now.AddDays(10);

        _mockTimeProvider.Setup(_ => _.Now()).Returns(nowDateTime);

        var newsWithSunriseDateInFuture = new ContentfulNewsBuilder().SunriseDate(futureSunRiseDate).Slug(slug).Build();
        ContentfulCollection<ContentfulNews> collection = new() {
            Items = new List<ContentfulNews> { newsWithSunriseDateInFuture }
        };

        var simpleNewsQuery = new QueryBuilder<ContentfulNews>()
            .ContentTypeIs("news")
            .FieldEquals("fields.slug", slug)
            .Include(1)
            .Build();

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns(newsWithSunriseDateInFuture.Body);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(collection.Items.ToList());

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void GetNews_ShouldReturnSuccessIfNewsArticleSunsetDateIsInThePast()
    {
        // Arrange
        const string slug = "news-with-sunrise-date-in-future";
        DateTime nowDateTime = DateTime.Now;
        DateTime pastSunRiseDate = DateTime.Now.AddDays(-20);
        DateTime pastSunSetDate = DateTime.Now.AddDays(-10);

        _mockTimeProvider.Setup(_ => _.Now()).Returns(nowDateTime);
        var newsWithSunsetDateInPast = new ContentfulNewsBuilder()
            .SunsetDate(pastSunSetDate)
            .SunriseDate(pastSunRiseDate)
            .Slug(slug)
            .Build();

        ContentfulCollection<ContentfulNews> collection = new() {
            Items = new List<ContentfulNews> { newsWithSunsetDateInPast }
        };

        var simpleNewsQuery = new QueryBuilder<ContentfulNews>()
            .ContentTypeIs("news")
            .FieldEquals("fields.slug", slug)
            .Include(1)
            .Build();

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns(newsWithSunsetDateInPast.Body);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(collection.Items.ToList());

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetNews(slug));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void ShouldReturnNewsOrderedBySunriseDate()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 08, 5));

        ContentfulNewsRoom contentfulNewsRoom = new() { Title = "test" };
        Newsroom newsRoom = new(_alerts, true, "test-id");
        _newsRoomContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNewsRoom>())).Returns(newsRoom);

        News news = new(Title, Slug, Teaser, Purpose, Image, ThumbnailImage, Body, _sunriseDate, _sunsetDate, _updatedAt, _crumbs, _alerts, new List<string>() { "tag1", "tag2" }, new List<Document>(), _newsCategories, new List<Profile>());

        _newsContentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulNews>())).Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new() {
            Items = new List<ContentfulNews> {
                new ContentfulNewsBuilder().Title("Oldest news article").Slug("old-news").Teaser("This is news").SunriseDate(new DateTime(2016, 01, 01, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("middle news article").Slug("middle-news").Teaser("This is news").SunriseDate(new DateTime(2016, 01, 02, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
                new ContentfulNewsBuilder().Title("newsest news article").Slug("new-news").Teaser("This is news").SunriseDate(new DateTime(2016, 01, 03, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
            }
        };

        _client.Setup(_ => _.GetEntries<ContentfulNews>(
                    It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsListCollection);

        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(newsListCollection.Items.ToList());
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(contentfulNewsRoom);
        _cacheWrapper.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-categories"), It.IsAny<Func<Task<List<string>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<string> { "Benefits", "foo", "Council leader" });

        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns("The news");

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null));

        // Assert
        var newsroom = response.Get<Newsroom>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        newsroom.News[0].Slug.Equals("new-news");
        newsroom.News[1].Slug.Equals("middle-news");
        newsroom.News[2].Slug.Equals("old-news");


        Assert.Equal(3, newsroom.News.Count);
        Assert.Equal("new-news", newsroom.News[0].Slug);
        Assert.Equal("middle-news", newsroom.News[1].Slug);
        Assert.Equal("old-news", newsroom.News[2].Slug);
    }
}
