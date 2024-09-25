﻿using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Repositories;

public class LandingPageRepositoryTests
{
    private readonly LandingPageRepository _repository;
    private readonly Mock<EventRepository> _eventRepository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulLandingPage, LandingPage>> _contentfulFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulNews, News>> _newsFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
    private readonly Mock<ITimeProvider> _timeprovider;
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IConfiguration> _configuration;

    public LandingPageRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _timeprovider = new Mock<ITimeProvider>();
        _eventHomepageFactory = new Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>();
        _timeprovider.Setup(o => o.Now()).Returns(DateTime.Today.AddDays(1));
        _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
        _cacheWrapper = new Mock<ICache>();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(_ => _.GetClient(config)).Returns(_contentfulClient.Object);

        Mock<ILogger<EventRepository>> _logger = new();
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

        _eventRepository = new(config, contentfulClientManager.Object, _timeprovider.Object, _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object, _logger.Object, _configuration.Object);

        _repository = new LandingPageRepository(config, _contentfulFactory.Object, contentfulClientManager.Object, _eventRepository.Object, _newsFactory.Object, _profileFactory.Object);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsSuccessResponse_WhenLandingPageIsFound()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new() { };
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(new LandingPage());

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsNotFoundResponse_WhenLandingPageIsNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = Enumerable.Empty<ContentfulLandingPage>() };
        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        // Act
        HttpResponse response = await _repository.GetLandingPage("non-existent-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No Landing Page found", response.Error);
    }

    [Fact]
    public async Task GetLandingPage_PopulatesContentBlocks_WhenPageSectionsItemsArePresent()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
            {
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build()
            },
        };

        ContentBlock contentBlock1 = new() { Title = "ContentBlock 1" };
        ContentBlock contentBlock2 = new() { Title = "ContentBlock 2" };

        LandingPage landingPage = new()
        {
            Slug = "landing-page-slug",
            Title = "landing page title",
            Subtitle = "landing page subtitle",
            Breadcrumbs = new List<Crumb>(),
            Alerts = new List<Alert>(),
            Teaser = "landing page teaser",
            MetaDescription = "landing page metadescription",
            Image = new MediaAsset(),
            Icon = "icon",
            HeaderType = "full image",
            HeaderImage = new MediaAsset(),
            PageSections = new List<ContentBlock>() { contentBlock1, contentBlock2 }
        };

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(landingPage);

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulReference>
            {
                Items = new List<ContentfulReference>
                {
                    new() { Sys = new SystemProperties { Id = "content-block-1" } },
                    new() { Sys = new SystemProperties { Id = "content-block-2" } }
                }
            });

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");
        LandingPage responseLandingPage = response.Get<LandingPage>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseLandingPage.PageSections);
        Assert.Equal(2, responseLandingPage.PageSections.Count());
        Assert.Equal(contentBlock1, responseLandingPage.PageSections.ToList()[0]);
        Assert.Equal(contentBlock2, responseLandingPage.PageSections.ToList()[1]);
    }

    [Fact]
    public async Task GetLandingPage_GetsEventsFromCategory_WhenCategoryEventListIsNotEmpty()
    {
        // Arrange
        LandingPage landingPage = new()
        {
            PageSections = new List<ContentBlock>
            {
                new() { ContentType = "EventCards", AssociatedTagCategory = "events" }
            }
        };

        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
             {
                 new ContentfulReferenceBuilder().Build(),
                 new ContentfulReferenceBuilder().Build(),
                 new ContentfulReferenceBuilder().Build()
             }
        };

        Event anyEvent = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), DateTime.MaxValue, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), EventFrequency.Weekly, new(), It.IsAny<string>(), new(), new List<string>(), It.IsAny<MapPosition>(), It.IsAny<bool>(), It.IsAny<string>(), DateTime.MinValue, new(), It.IsAny<Group>(), new(), new(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>());

        List<Event> events = new() { anyEvent, anyEvent, anyEvent };

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(new ContentfulCollection<ContentfulReference>
         {
             Items = new List<ContentfulReference>
             {
                new () { Sys = new SystemProperties { Id = "content-block-1" }},
                new() { Sys = new SystemProperties { Id = "content-block-2" }}
             }
         });

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };
        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
        _contentfulFactory.Setup(factory => factory.ToModel(It.IsAny<ContentfulLandingPage>()))
            .Returns(landingPage);
        _eventRepository.Setup(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(events);

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");
        List<Event> responseEvents = response.Get<LandingPage>().PageSections.First().Events;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _eventRepository.Verify(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _eventRepository.Verify(repository => repository.GetEventsByTag(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        Assert.NotNull(responseEvents);
        Assert.Equal(3, responseEvents.Count);
    }

    [Fact]
    public async Task GetLandingPage_GetsEventsFromTags_WhenCategoryEventListIsEmpty()
    {
        // Arrange
        LandingPage landingPage = new()
        {
            PageSections = new List<ContentBlock>
            {
                new() { ContentType = "EventCards", AssociatedTagCategory = "events" }
            }
        };

        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
             {
                 new ContentfulReferenceBuilder().Build(),
                 new ContentfulReferenceBuilder().Build(),
                 new ContentfulReferenceBuilder().Build()
             }
        };

        Event anyEvent = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), DateTime.MaxValue, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), EventFrequency.Weekly, new(), It.IsAny<string>(), new(), new List<string>(), It.IsAny<MapPosition>(), It.IsAny<bool>(), It.IsAny<string>(), DateTime.MinValue, new(), It.IsAny<Group>(), new(), new(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>());

        List<Event> emptyEvents = new();
        List<Event> tagEvents = new() { anyEvent, anyEvent, anyEvent };

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulReference>
            {
                Items = new List<ContentfulReference>
                {
                    new () { Sys = new SystemProperties { Id = "content-block-1" } },
                    new() { Sys = new SystemProperties { Id = "content-block-2" } }
                }
            });

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };
        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
        _contentfulFactory.Setup(factory => factory.ToModel(It.IsAny<ContentfulLandingPage>()))
            .Returns(landingPage);
        _eventRepository.Setup(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(emptyEvents);
        _eventRepository.Setup(repository => repository.GetEventsByTag(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(tagEvents);

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");
        List<Event> responseEvents = response.Get<LandingPage>().PageSections.First().Events;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _eventRepository.Verify(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _eventRepository.Verify(repository => repository.GetEventsByTag(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        Assert.NotNull(responseEvents);
        Assert.Equal(3, responseEvents.Count);
    }

    [Fact]
    public async Task GetLandingPage_ShouldCallGetLatestNewsByCategory_WhenNewsBannerExists()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
            {
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build()
            },
        };

        LandingPage landingPage = new()
        {
            Slug = "existing-slug",
            PageSections = new List<ContentBlock>
            {
                new()
                {
                    ContentType = "NewsBanner",
                    AssociatedTagCategory = "news-category"
                }
            }
        };

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        News news = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<Crumb>>(),
            It.IsAny<List<Alert>>(), It.IsAny<List<string>>(), It.IsAny<List<Document>>(), It.IsAny<List<string>>(), It.IsAny<List<Profile>>());

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
           It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(landingPage);

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulReference>
            {
                Items = new List<ContentfulReference>
                {
                    new() { Sys = new SystemProperties { Id = "content-block-1" } },
                    new() { Sys = new SystemProperties { Id = "content-block-2" } }
                }
            });

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = new List<ContentfulNews> { new() } });

        _newsFactory.Setup(factory => factory.ToModel(new ContentfulNews()))
            .Returns(news);

        // Act
        HttpResponse response = await _repository.GetLandingPage("existing-slug");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLandingPage_ShouldCallGetProfile_WhenProfileBannerExists()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
            {
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build()
            },
        };

        LandingPage landingPage = new()
        {
            Slug = "existing-slug",
            PageSections = new List<ContentBlock>
            {
                new()
                {
                    ContentType = "ProfileBanner",
                    SubItems = new List<ContentBlock>() {
                        new() {}
                    }
                }
            }
        };

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        ContentfulProfile contentfulProfile = new();
        Profile profile = new();

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
           It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(landingPage);

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulReference>
            {
                Items = new List<ContentfulReference>
                {
                    new() { Sys = new SystemProperties { Id = "content-block-1" } },
                    new() { Sys = new SystemProperties { Id = "content-block-2" } }
                }
            });

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<ContentfulProfile> { Items = new List<ContentfulProfile> { new() } });

        _profileFactory.Setup(factory => factory.ToModel(new ContentfulProfile()))
            .Returns(profile);

        // Act
        HttpResponse response = await _repository.GetLandingPage("existing-slug");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestNewsByCategory_ShouldReturnNews_WhenCategoryMatches()
    {
        // Arrange
        ContentfulNews contentfulNews = new();
        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = new List<ContentfulNews> { contentfulNews } });

        // Act
        ContentfulNews news = await _repository.GetLatestNewsByCategory("some-category");

        // Assert
        Assert.NotNull(news);
        Assert.Equal(contentfulNews, news);
        _contentfulClient.Verify(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("tags")), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetLatestNewsByTag_ShouldReturnNews_WhenTagMatches()
    {
        // Arrange
        ContentfulNews contentfulNews = new();
        _contentfulClient.Setup(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("tags")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = new List<ContentfulNews> { contentfulNews } });

        // Act
        ContentfulNews news = await _repository.GetLatestNewsByTag("some-tag");

        // Assert
        Assert.NotNull(news);
        Assert.Equal(contentfulNews, news);
        _contentfulClient.Verify(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("tags")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProfile_ReturnsProfile_WhenProfileExists()
    {
        // Arrange
        string slug = "test-slug";
        ContentfulProfile profile = new() { Slug = slug };
        ContentfulCollection<ContentfulProfile> profiles = new()
        {
            Items = new List<ContentfulProfile> { profile }
        };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profiles);

        // Act
        ContentfulProfile result = await _repository.GetProfile(slug);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slug, result.Slug);
    }

    [Fact]
    public async Task GetProfile_ReturnsNull_WhenNoProfileExists()
    {
        // Arrange
        ContentfulCollection<ContentfulProfile> profiles = new() { Items = new List<ContentfulProfile>() };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profiles);

        // Act
        ContentfulProfile result = await _repository.GetProfile("non-existent-slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProfile_ThrowsException_WhenContentfulClientFails()
    {
        // Arrange
        string slug = "test-slug";
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Contentful service error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetProfile(slug));
    }
}