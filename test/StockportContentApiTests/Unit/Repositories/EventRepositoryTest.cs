namespace StockportContentApiTests.Unit.Repositories;

public class EventRepositoryTest
{
    private readonly EventRepository _repository;
    private readonly Mock<ITimeProvider> _mockTimeProvider = new Mock<ITimeProvider>();
    private readonly Mock<IContentfulClient> _contentfulClient = new Mock<IContentfulClient>();
    private readonly Mock<IHttpClient> _httpClient = new Mock<IHttpClient>();
    private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _eventCategoryFactory = new Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>>();
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
    private readonly Mock<ILogger<EventRepository>> _logger = new Mock<ILogger<EventRepository>>();
    private readonly Mock<ICache> _cacheWrapper = new Mock<ICache>();
    private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new Mock<IContentfulClientManager>();
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory = new Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>();
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();

    public EventRepositoryTest()
    {
        // Arrange
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        // Mock
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "subHeading", "body",
                                                             "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                             new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), string.Empty, false, string.Empty));

        // TODO: Make this into a mock instead of concrete class, will need refactor to tests with this also
        var contentfulFactory = new EventContentfulFactory(_documentFactory.Object, _groupFactory.Object, _eventCategoryFactory.Object, _alertFactory.Object, _mockTimeProvider.Object);
        var eventHomepageFactory = new EventHomepageContentfulFactory(_mockTimeProvider.Object);

        _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_contentfulClient.Object);
        _eventCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulEventCategory>())).Returns(new EventCategory("", "", ""));
        _configuration.Setup(_ => _["redisExpiryTimes:Articles"]).Returns("60");
        _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

        _repository = new EventRepository(_config, _contentfulClientManager.Object,
            _mockTimeProvider.Object, contentfulFactory, eventHomepageFactory, _cacheWrapper.Object, _logger.Object, _configuration.Object);
    }

    [Fact]
    public void GetReoccuringEventsNextEventOnly()
    {
        // Arrange - mock reoccurring event, starting in the past that passes today
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 07, 01));
        var rawEvent = new ContentfulEventBuilder().EventCategory(new List<string>() { "category" }).EventDate(new DateTime(2017, 06, 01)).Occurrences(10).Frequency(EventFrequency.Weekly).Build();
        var events = new List<ContentfulEvent> { rawEvent };

        _eventCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulEventCategory>())).Returns(new EventCategory("category", "category", "icon"));

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act - return events using a method which checks occurances
        var result = AsyncTestHelper.Resolve(_repository.GetEventsByCategory("category", true));

        // Assert - Check event date is first date that occurs in the future
        result[0].EventDate.Should().Be(new DateTime(2017, 07, 06));
    }

    [Fact]
    public void GetsASingleEventItemFromASlug()
    {
        // Arrange
        const string slug = "event-of-the-century";
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));

        var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();

        var events = new List<ContentfulEvent> { rawEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventItem = response.Get<Event>();
        eventItem.Slug.Should().Be(slug);
    }

    [Fact]
    public void GetsParticularReccuringEventFromASlug()
    {
        const string slug = "event-of-the-century";
        const int occurences = 3;
        const EventFrequency frequency = EventFrequency.Daily;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));

        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Slug(slug)
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();
        var anotherEvent = new ContentfulEventBuilder().Slug("slug-2").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        var aThirdEvent = new ContentfulEventBuilder().Slug("slug-3").Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        var collection = new ContentfulCollection<ContentfulEvent>();
        collection.Items = new List<ContentfulEvent> { anEvent };


        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(2);
        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 04, 02)));
        var eventItem = response.Get<Event>();

        eventItem.EventDate.Should().Be(new DateTime(2017, 04, 02));
    }

    [Fact]
    public void GetsA404ForANotFoundEventItem()
    {
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 08, 5));

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

        var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-not-found", new DateTime(2017, 4, 1)));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be("No event found for 'event-not-found'");
    }

    [Fact]
    public void ShouldGetAllEvents()
    {
        // Arrange
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
        var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 09, 08)).Build();
        var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 10, 08)).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent };
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(2);
    }

    [Fact]
    public void ShouldGetEventsIfContainingGroupIsNotArchived()
    {
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
        var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 09, 08)).Build();
        var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 10, 08)).Build();
        var archivedvents = new ContentfulEventBuilder().EventDate(new DateTime(2016, 10, 08)).Build();
        archivedvents.Group.DateHiddenFrom = DateTime.Now.AddDays(-2);
        archivedvents.Group.DateHiddenTo = DateTime.Now.AddDays(2);
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, archivedvents };

        var newsListCollection = new ContentfulCollection<ContentfulEvent>();
        newsListCollection.Items = new List<ContentfulEvent>
        {
            anEvent,
            anotherEvent,
            archivedvents
        };

        var builder = new QueryBuilder<CancellationToken>().ContentTypeIs("events").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);

        _contentfulClient
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        var contentfulEvents = AsyncTestHelper.Resolve(_repository.GetAllEvents());

        contentfulEvents.Count.Should().Be(2);
        contentfulEvents.First()
            .Should().BeEquivalentTo(anEvent);

        contentfulEvents.Last()
            .Should().BeEquivalentTo(anotherEvent);
    }

    [Fact]
    public void ShouldGet404IfContentNotFound()
    {
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

        _contentfulClient.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<Event>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<Event>());

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be("No events found");
    }

    [Fact]
    public void ShouldGetAllEventsWithTheirDailyReccuringParts()
    {
        const int occurences = 3;
        const EventFrequency frequency = EventFrequency.Daily;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();
        var events = new List<ContentfulEvent> { anEvent };
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));

        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(occurences);
        eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
        eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 04, 02));
        eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 04, 03));
    }

    [Fact]
    public void ShouldGetAllEventsWithTheirWeeklyReccuringParts()
    {
        const int occurences = 4;
        const EventFrequency frequency = EventFrequency.Weekly;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();

        var events = new List<ContentfulEvent> { anEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));

        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(occurences);
        eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
        eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 04, 08));
        eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 04, 15));
        eventCalender.Events[3].EventDate.Should().Be(new DateTime(2017, 04, 22));
    }

    [Fact]
    public void ShouldGetWeeklyRepeatedDatesBetweenDates()
    {
        const int occurences = 4;
        const EventFrequency frequency = EventFrequency.Weekly;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();
        var events = new List<ContentfulEvent> { anEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response =
            AsyncTestHelper.Resolve(_repository.Get(new DateTime(2017, 04, 01), new DateTime(2017, 04, 16), null, 0, null, null, null, 0, 0));

        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(3);
        eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
        eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 04, 08));
        eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 04, 15));
    }

    [Fact]
    public void ShouldGetAllEventsWithTheirForthnightlyReccuringParts()
    {
        const int occurences = 3;
        const EventFrequency frequency = EventFrequency.Fortnightly;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();
        var events = new List<ContentfulEvent> { anEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));


        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(occurences);
        eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
        eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 04, 15));
        eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 04, 29));
    }

    [Fact]
    public void ShouldGetAllEventsWithTheirMonthlyReccuringParts()
    {
        const int occurences = 5;
        const EventFrequency frequency = EventFrequency.Monthly;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();
        var events = new List<ContentfulEvent> { anEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));


        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(occurences);
        eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
        eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 05, 01));
        eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 06, 01));
        eventCalender.Events[3].EventDate.Should().Be(new DateTime(2017, 07, 01));
    }

    [Fact]
    public void ShouldGetAllEventsWithTheirYearlyReccuringParts()
    {
        const int occurences = 3;
        const EventFrequency frequency = EventFrequency.Yearly;
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

        var anEvent =
            new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                .Frequency(frequency)
                .Occurrences(occurences)
                .Build();
        var events = new List<ContentfulEvent> { anEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));


        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Count.Should().Be(occurences);
        eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
        eventCalender.Events[1].EventDate.Should().Be(new DateTime(2018, 04, 01));
        eventCalender.Events[2].EventDate.Should().Be(new DateTime(2019, 04, 01));
    }

    [Fact]
    public void ShouldGetEventsWithinDateRange()
    {
        // Arrange
        var dateFrom = new DateTime(2016, 07, 28);
        var dateTo = new DateTime(2017, 02, 15);

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
        var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 09, 01)).Build();
        var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Build();

        var events = new List<ContentfulEvent> { anEvent, anotherEvent };
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo, null, 0, null, null, null, 0, 0));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldGetOneEventForACategory()
    {
        // Arrange
        var contentfulCategory1 = new ContentfulEventCategory { Name = "Category 1", Slug = "category-1" };
        var contentfulCategory2 = new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" };
        var category1 = new EventCategory("Category 1", "category-1", "icon1");
        var category2 = new EventCategory("Category 2", "category-2", "icon2");
        var anEvent =
            new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { contentfulCategory1, contentfulCategory2 }).Build();
        var anotherEvent = new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" } }).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent };

        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory1)).Returns(category1);
        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory2)).Returns(category2);

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, "category 1", 0, null, null, null, 0, 0));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldGetTwoEventsForACategory()
    {
        // Arrange
        var contentfulCategory1 = new ContentfulEventCategory { Name = "category 1", Slug = "category 1" };
        var contentfulCategory2 = new ContentfulEventCategory { Name = "category 2", Slug = "category 2" };
        var category1 = new EventCategory("category 1", "category 1", "icon1");
        var category2 = new EventCategory("category 2", "category 2", "icon2");
        var anEvent =
            new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory> { contentfulCategory1, contentfulCategory2 }).Build();
        var anotherEvent = new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory> { contentfulCategory2 }).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent };


        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory1)).Returns(category1);
        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory2)).Returns(category2);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, "category 2", 0, null, null, null, 0, 0));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldGetEventForACategoryAndDate()
    {
        // Arrange
        var dateFrom = new DateTime(2016, 07, 28);
        var dateTo = new DateTime(2017, 02, 15);

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
        var contentfulCategory1 = new ContentfulEventCategory { Name = "Category 1", Slug = "category-1" };
        var contentfulCategory2 = new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" };
        var contentfulCategory3 = new ContentfulEventCategory { Name = "Category 3", Slug = "category-3" };

        var event1 =
            new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { contentfulCategory1 })
                .EventDate(new DateTime(2017, 08, 01))
                .Build();
        var event2 =
            new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { contentfulCategory2 })
                .EventDate(new DateTime(2016, 08, 01))
                .Build();
        var event3 =
            new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { contentfulCategory3 })
                .EventDate(new DateTime(2016, 08, 01))
                .Build();

        var events = new List<ContentfulEvent> { event1, event2, event3 };

        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory1)).Returns(new EventCategory("Category 1", "category-1", "icon1"));
        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory2)).Returns(new EventCategory("Category 2", "category-2", "icon2"));
        _eventCategoryFactory.Setup(o => o.ToModel(contentfulCategory3)).Returns(new EventCategory("Category 3", "category-3", "icon3"));

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo, "Category 3", 0, null, null, null, 0, 0));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventCalender = response.Get<EventCalender>();
        eventCalender.Events.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldGetTwoFeaturedEvents()
    {
        var anEvent = new ContentfulEventBuilder().Featured(true).EventDate(new DateTime(2017, 09, 01)).Build();
        var anotherEvent = new ContentfulEventBuilder().Featured(true).EventDate(new DateTime(2017, 10, 10)).Build();
        var aThirdEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 2, true, null, null, 0, 0));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventsCalendar = response.Get<EventCalender>();

        eventsCalendar.Events[0].Featured.Should().BeTrue();
        eventsCalendar.Events[1].Featured.Should().BeTrue();
    }

    [Fact]
    public void ShouldGetOneFeaturedEventAndOneNoneFeatured()
    {
        var anEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        var anotherEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 10, 10)).Build();
        var aThirdEvent = new ContentfulEventBuilder().Featured(true).EventDate(new DateTime(2017, 09, 15)).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 2, true, null, null, 0, 0));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventsCalendar = response.Get<EventCalender>();

        eventsCalendar.Events[0].Featured.Should().BeTrue();
        eventsCalendar.Events[1].Featured.Should().BeFalse();
    }

    [Fact]
    public void ShouldGetNoFeaturedEventsBecauseNonExist()
    {
        var anEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        var anotherEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 10, 10)).Build();
        var aThirdEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 2, true, null, null, 0, 0));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventsCalendar = response.Get<EventCalender>();

        eventsCalendar.Events[0].Featured.Should().BeFalse();
        eventsCalendar.Events[1].Featured.Should().BeFalse();
    }


    [Fact]
    public void ShouldGetLinkEventsForAGroup()
    {
        var anEvent = new ContentfulEventBuilder().Slug("slug-1").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        var anotherEvent = new ContentfulEventBuilder().Slug("slug-2").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        var aThirdEvent = new ContentfulEventBuilder().Slug("slug-3").Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var group = new GroupBuilder().Slug("zumba-fitness").Build();

        _groupFactory.Setup(g => g.ToModel(It.IsAny<ContentfulGroup>())).Returns(group);

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));

        var response = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("zumba-fitness"));

        response[0].Title.Should().Be("title");
        response[1].Description.Should().Be("description");

    }

    [Fact]
    public void ShouldReturnNoEventsIfTheGroupIsReferencedByNone()
    {
        // Arrange
        var anEvent = new ContentfulEventBuilder().Slug("slug-1").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        anEvent.Group.Slug = "kersal-rugby";
        var anotherEvent = new ContentfulEventBuilder().Slug("slug-2").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        anotherEvent.Group.Slug = "kersal-rugby";
        var aThirdEvent = new ContentfulEventBuilder().Slug("slug-3").Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        aThirdEvent.Group.Slug = "kersal-rugby";
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var group = new GroupBuilder().Slug("zumba-fitness").Build();
        var othergroup = new GroupBuilder().Slug("kersal-rugby").Build();

        _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug == "zumba-fitness"))).Returns(group);
        _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug != "zumba-fitness"))).Returns(othergroup);

        // Act
        var processedEvents = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("zumba-fitness"));

        // Assert
        processedEvents.Should().BeEmpty();
    }

    [Fact]
    public void ShouldReturnOneEventIfTheGroupIsReferencedByOne()
    {
        // Arrange
        var anEvent = new ContentfulEventBuilder().Slug("slug-1").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        anEvent.Group.Slug = "zumba-fitness";
        var anotherEvent = new ContentfulEventBuilder().Slug("slug-2").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        anotherEvent.Group.Slug = "kersal-rugby";
        var aThirdEvent = new ContentfulEventBuilder().Slug("slug-3").Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        aThirdEvent.Group.Slug = "kersal-rugby";
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var group = new GroupBuilder().Slug("zumba-fitness").Build();
        var othergroup = new GroupBuilder().Slug("kersal-rugby").Build();

        _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug == "zumba-fitness"))).Returns(group);
        _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug != "zumba-fitness"))).Returns(othergroup);

        // Act
        var processedEvents = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("zumba-fitness"));

        // Assert
        processedEvents.Count.Should().Be(1);
        processedEvents[0].Slug.Should().Be("slug-1");
    }

    [Fact]
    public void ShouldReturnThreeEventsIfTheGroupIsReferencedByThree()
    {
        // Arrange
        var anEvent = new ContentfulEventBuilder().Slug("slug-1").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        anEvent.Group.Slug = "zumba-fitness";
        var anotherEvent = new ContentfulEventBuilder().Slug("slug-2").Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        anotherEvent.Group.Slug = "zumba-fitness";
        var aThirdEvent = new ContentfulEventBuilder().Slug("slug-3").Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
        aThirdEvent.Group.Slug = "zumba-fitness";
        var events = new List<ContentfulEvent> { anEvent, anotherEvent, aThirdEvent };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        var group = new GroupBuilder().Slug("zumba-fitness").Build();
        var othergroup = new GroupBuilder().Slug("kersal-rugby").Build();

        _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug == "zumba-fitness"))).Returns(group);
        _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug != "zumba-fitness"))).Returns(othergroup);

        // Act
        var processedEvents = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("zumba-fitness"));

        // Assert
        processedEvents.Count.Should().Be(3);
        processedEvents[0].Slug.Should().Be("slug-1");
    }

    [Fact]
    public async void ShouldReturn404IfNoEventsFoundInGetByCategory()
    {
        //Arrange
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync((List<ContentfulEvent>)null);

        //Act
        var result = await _repository.Get("Blah");

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void ShouldReturnEventsAsFoundInGetByCategory()
    {
        // Arrange
        var events = new List<ContentfulEvent> { new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build() };
        var theEvent = new EventBuilder().EventCategories(new List<EventCategory> { new EventCategory("category", "category", "category") }).Build();

        // Mock
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);
        _eventFactory.Setup(g => g.ToModel(It.IsAny<ContentfulEvent>())).Returns(theEvent);

        // Act
        var repository = new EventRepository(_config, _contentfulClientManager.Object, _mockTimeProvider.Object, _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object, _logger.Object, _configuration.Object);
        var result = await repository.Get("category");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventResponse = result.Get<List<Event>>();
        eventResponse.Count.Should().Be(3);
    }

    [Fact]
    public async void ShouldReturnEventsByTagIfNoCategoryPresent()
    {
        // Arrange
        var events = new List<ContentfulEvent> { new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build() };
        var theEvent = new EventBuilder().Tags(new List<string> { "category-tag" }).Build();

        // Mock
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);
        _eventFactory.Setup(g => g.ToModel(It.IsAny<ContentfulEvent>())).Returns(theEvent);

        // Act
        var repository = new EventRepository(_config, _contentfulClientManager.Object, _mockTimeProvider.Object, _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object, _logger.Object, _configuration.Object);
        var result = await repository.Get("category-tag");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventResponse = result.Get<List<Event>>();
        eventResponse.Count.Should().Be(3);
    }

    [Fact]
    public void EventsShouldReturnInOrderOfStartDate()
    {

        //Arrange 
        var firstEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
        var secondEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 02)).StartTime("8:00").Build();
        var thirdEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 02)).StartTime("8:30").Build();
        var events = new List<ContentfulEvent> { firstEvent, secondEvent, thirdEvent };

        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 3, true, null, null, 0, 0));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventsCalendar = response.Get<EventCalender>();

        // Assert
        Assert.Equal(eventsCalendar.Events[0].EventDate, firstEvent.EventDate);
        Assert.Equal(eventsCalendar.Events[0].StartTime, firstEvent.StartTime);

        Assert.Equal(eventsCalendar.Events[1].EventDate, secondEvent.EventDate);
        Assert.Equal(eventsCalendar.Events[1].StartTime, secondEvent.StartTime);

        Assert.Equal(eventsCalendar.Events[2].EventDate, thirdEvent.EventDate);
        Assert.Equal(eventsCalendar.Events[2].StartTime, thirdEvent.StartTime);
    }
}

