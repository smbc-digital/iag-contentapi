using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;
using StockportContentApiTests.Unit.Builders;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventRepositoryTest
    {
        private readonly EventRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly Mock<IContentfulFactory<List<ContentfulEventCategory>, List<EventCategory>>> _eventCategoryListFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
        private readonly Mock<ILogger<EventRepository>> _logger;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<IConfiguration> _configuration;

        public EventRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            var documentFactory = new DocumentContentfulFactory(HttpContextFake.GetHttpContextFake());
            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _eventCategoryListFactory = new Mock<IContentfulFactory<List<ContentfulEventCategory>, List<EventCategory>>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "subHeading", "body",
                                                                 "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                                 new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)));

            _mockTimeProvider = new Mock<ITimeProvider>();
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            var contentfulFactory = new EventContentfulFactory(documentFactory, _groupFactory.Object, _eventCategoryListFactory.Object, _alertFactory.Object, _mockTimeProvider.Object, HttpContextFake.GetHttpContextFake());
            var eventHomepageFactory = new EventHomepageContentfulFactory(_mockTimeProvider.Object);
            _httpClient = new Mock<IHttpClient>();
            
            _logger = new Mock<ILogger<EventRepository>>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _eventCategoryListFactory.Setup(o => o.ToModel(It.IsAny<List<ContentfulEventCategory>>())).Returns(new List<EventCategory>());

            _cacheWrapper = new Mock<ICache>();

            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:Articles"]).Returns("60");
            _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

            _repository = new EventRepository(config, contentfulClientManager.Object,
                _mockTimeProvider.Object, contentfulFactory, eventHomepageFactory, _cacheWrapper.Object, _logger.Object, _configuration.Object);
        }

        [Fact]
        public void GetReoccuringEventsNextEventOnly()
        {
            // Arrange - mock reoccurring event, starting in the past that passes today
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 07, 01));
            var rawEvent = new ContentfulEventBuilder().EventCategory(new List<string>() { "category" }).EventDate(new DateTime(2017, 06, 01)).Occurrences(10).Frequency(EventFrequency.Weekly).Build();
            var events = new List<ContentfulEvent> { rawEvent };

            _eventCategoryListFactory.Setup(o => o.ToModel(It.IsAny<List<ContentfulEventCategory>>())).Returns(new List<EventCategory>() { new EventCategory("category", "category", "icon") });

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            // Act - return events using a method which checks occurances
            var result = AsyncTestHelper.Resolve(_repository.GetEventsByCategory("category"));

            // Assert - Check event date is first date that occurs in the future
            result[0].EventDate.Should().Be(new DateTime(2017, 07, 06));
        }

        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            const string slug = "event-of-the-century";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();

            var events = new List<ContentfulEvent> { rawEvent };

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();

            eventItem.ShouldBeEquivalentTo(rawEvent,
                o =>
                    o.Excluding(raw => raw.ThumbnailImageUrl)
                        .Excluding(raw => raw.ImageUrl)
                        .Excluding(raw => raw.Documents)
                        .Excluding(e => e.UpdatedAt)
                        .Excluding(e => e.Group)
                        .Excluding(e => e.Coord)
                        .Excluding(e => e.Alerts)
                        .Excluding(e => e.EventCategories)
                        .Excluding(e => e.Breadcrumbs)
                        .Excluding(e => e.EventFrequency));

            eventItem.ThumbnailImageUrl.Should().Be(rawEvent.Image.File.Url + "?h=250");
            eventItem.ImageUrl.Should().Be(rawEvent.Image.File.Url);
            eventItem.Documents.Count.Should().Be(rawEvent.Documents.Count);
            eventItem.BookingInformation.Should().Be(rawEvent.BookingInformation);
            eventItem.Alerts.Count.Should().Be(1);
            eventItem.Alerts[0].Title.Should().Be(rawEvent.Alerts[0].Title);
            eventItem.Featured.Should().BeFalse();
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
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
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
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 09, 08)).Build();
            var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 10, 08)).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent};

            var builder = new QueryBuilder<CancellationToken>().ContentTypeIs("events").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null, null, 0, 0));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Count.Should().Be(2);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o
                    .Excluding(e => e.ThumbnailImageUrl)
                    .Excluding(e => e.ImageUrl)
                    .Excluding(e => e.Documents)
                    .Excluding(e => e.UpdatedAt)
                    .Excluding(e => e.Coord)
                    .Excluding(e => e.Group)
                    .Excluding(e => e.EventCategories)
                    .Excluding(e => e.Alerts)
                    .Excluding(e => e.Breadcrumbs)
                    .Excluding(e => e.EventFrequency));

            eventCalender.Events.Last()
                .ShouldBeEquivalentTo(anotherEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl)
                    .Excluding(e => e.ImageUrl)
                    .Excluding(e => e.Documents)
                    .Excluding(e => e.UpdatedAt)
                    .Excluding(e => e.Coord)
                    .Excluding(e => e.Group)
                    .Excluding(e => e.EventCategories)
                    .Excluding(e => e.Alerts)
                    .Excluding(e => e.Breadcrumbs)
                    .Excluding(e => e.EventFrequency));
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

           _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(
                It.Is<string>(q => q.Contains(new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2).Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);           

            var contentfulEvents = AsyncTestHelper.Resolve(_repository.GetAllEvents());

            contentfulEvents.Count.Should().Be(2);
            contentfulEvents.First()
                .ShouldBeEquivalentTo(anEvent);

            contentfulEvents.Last()
                .ShouldBeEquivalentTo(anotherEvent);
        }

        [Fact]
        public void ShouldGet404IfContentNotFound()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.IsAny<QueryBuilder<Event>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<Event>());

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null, null, 0, 0));

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
            var events = new List<ContentfulEvent> {anEvent};
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null, null, 0, 0));

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

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null, null, 0, 0));

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
            var events = new List<ContentfulEvent> {anEvent};

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response =
                AsyncTestHelper.Resolve(_repository.Get(new DateTime(2017, 04, 01), new DateTime(2017, 04, 16), null,  0, null, null, null, 0, 0));

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
            var events = new List<ContentfulEvent> {anEvent};

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  0, null, null, null, 0, 0));


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
            var events = new List<ContentfulEvent> {anEvent};

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null, null, 0, 0));


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
            var events = new List<ContentfulEvent> {anEvent};

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  0, null, null, null, 0, 0));


            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Count.Should().Be(occurences);
            eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
            eventCalender.Events[1].EventDate.Should().Be(new DateTime(2018, 04, 01));
            eventCalender.Events[2].EventDate.Should().Be(new DateTime(2019, 04, 01));
        }

        [Fact]
        public void ShouldGetEventsWithinDateRange()
        {
            var dateFrom = new DateTime(2016, 07, 28);
            var dateTo = new DateTime(2017, 02, 15);

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 09, 01)).Build();
            var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Build();

            var events = new List<ContentfulEvent> {anEvent, anotherEvent};
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo, null, 0, null, null, null, 0, 0));


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Coord).Excluding(e => e.Coord).Excluding(e => e.EventCategories).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts).Excluding(e => e.Breadcrumbs).Excluding(e => e.EventFrequency).Excluding(e => e.EventFrequency));
        }

        [Fact]
        public void ShouldGetOneEventForACategory()
        {
            var anEvent =
                new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 1", Slug = "category-1" }, new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" } }).Build();
            var anotherEvent = new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" } }).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent};

            _eventCategoryListFactory.Setup(o => o.ToModel(events[0].EventCategories)).Returns(new List<EventCategory>() { new EventCategory("Category 1", "category-1", "icon1"), new EventCategory("Category 2", "category-2", "icon2") });
            _eventCategoryListFactory.Setup(o => o.ToModel(events[1].EventCategories)).Returns(new List<EventCategory>() { new EventCategory("Category 2", "category-2", "icon2")});

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null,  "category 1", 0, null, null, null, 0, 0));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Coord).Excluding(e => e.EventCategories).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts).Excluding(e => e.Breadcrumbs).Excluding(e => e.EventFrequency));
        }

        [Fact]
        public void ShouldGetTwoEventsForACategory()
        {
            var anEvent =
                new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 1", Slug = "category-1" }, new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" } }).Build();
            var anotherEvent = new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" } }).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent};

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            _eventCategoryListFactory.Setup(o => o.ToModel(It.IsAny<List<ContentfulEventCategory>>())).Returns(new List<EventCategory>() { new EventCategory("Category 1", "category-1", "icon1"), new EventCategory("Category 2", "category-2", "icon2") });

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, "category 2", 0, null, null, null, 0, 0));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(2);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Coord).Excluding(e => e.EventCategories).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts).Excluding(e => e.Breadcrumbs).Excluding(e => e.Breadcrumbs).Excluding(e => e.EventFrequency));
        }

        [Fact]
        public void ShouldGetEventForACategoryAndDate()
        {
            var dateFrom = new DateTime(2016, 07, 28);
            var dateTo = new DateTime(2017, 02, 15);

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));                     

            var event1 =
                new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 1", Slug = "category-1" } })
                    .EventDate(new DateTime(2017, 08, 01))
                    .Build();
            var event2 =
                new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" } })
                    .EventDate(new DateTime(2016, 08, 01))
                    .Build();
            var event3 =
                new ContentfulEventBuilder().EventCategoryList(new List<ContentfulEventCategory>() { new ContentfulEventCategory { Name = "Category 3", Slug = "category-3" } })
                    .EventDate(new DateTime(2016, 08, 01))
                    .Build();

            var events = new List<ContentfulEvent> {event1, event2, event3};

            _eventCategoryListFactory.Setup(o => o.ToModel(events[0].EventCategories)).Returns(new List<EventCategory>() { new EventCategory("Category 1", "category-1", "icon1") });
            _eventCategoryListFactory.Setup(o => o.ToModel(events[1].EventCategories)).Returns(new List<EventCategory>() { new EventCategory("Category 2", "category-2", "icon2") });
            _eventCategoryListFactory.Setup(o => o.ToModel(events[2].EventCategories)).Returns(new List<EventCategory>() { new EventCategory("Category 3", "category-3", "icon3") });
            
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo, "Category 3", 0, null, null, null, 0, 0));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(event3,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Coord).Excluding(e => e.EventCategories).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts).Excluding(e => e.Breadcrumbs).Excluding(e => e.EventFrequency));
        }

       
        public void ShouldGetEventsWithLimitOfTwo()
        {
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 09, 01)).Build();
            var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 10, 10)).Build();
            var eventThree = new ContentfulEventBuilder().EventDate(new DateTime(2017, 10, 11)).Build();

            var collection = new ContentfulCollection<ContentfulEvent>();
            collection.Items = new List<ContentfulEvent> { anEvent, anotherEvent, eventThree };

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));

            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 2, null, null, null, 0, 0));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventsCalendar = response.Get<EventCalender>();
            eventsCalendar.Events.Should().HaveCount(2);

            eventsCalendar.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.Group));
        }


        public void ShouldCreateEventWithFauteredSetToTrue()
        {
            const string slug = "event-of-the-century";
            var anEvent = new ContentfulEventBuilder().Featured(true).Build();
            var collection = new ContentfulCollection<ContentfulEvent>();
            collection.Items = new List<ContentfulEvent> { anEvent };

            _contentfulClient.Setup(
                o =>
                    o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(),
                        It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();
            eventItem.Featured.Should().BeTrue();
        }

        public void ShouldCreateEventWithFauteredSetToFalse()
        {
            const string slug = "event-of-the-century";
            var anEvent = new ContentfulEventBuilder().Featured(false).Build();
            var collection = new ContentfulCollection<ContentfulEvent>();
            collection.Items = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(
                o =>
                    o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(),
                        It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();
            eventItem.Featured.Should().BeTrue();
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

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  2, true, null, null, 0, 0));

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

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  2, true, null, null, 0, 0));

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
            var events = new List<ContentfulEvent> {anEvent, anotherEvent, aThirdEvent};

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            var group = new Group("name", "zumba-fitness", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

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

            var group = new Group("name", "zumba-fitness", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

            var othergroup = new Group("name", "kersal-rugby", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

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

            var group = new Group("name", "zumba-fitness", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

            var othergroup = new Group("name", "kersal-rugby", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

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

            var group = new Group("name", "zumba-fitness", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

            var othergroup = new Group("name", "kersal-rugby", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, null, false, null, DateTime.MinValue, DateTime.MaxValue, "published", string.Empty, string.Empty, string.Empty, string.Empty, null, false);

            _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug == "zumba-fitness"))).Returns(group);
            _groupFactory.Setup(g => g.ToModel(It.Is<ContentfulGroup>(o => o.Slug != "zumba-fitness"))).Returns(othergroup);

            // Act
            var processedEvents = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("zumba-fitness"));

            // Assert
            processedEvents.Count.Should().Be(3);
            processedEvents[0].Slug.Should().Be("slug-1");
        }
    }
}

