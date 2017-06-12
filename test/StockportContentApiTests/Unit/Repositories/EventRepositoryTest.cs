using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;
using StockportContentApiTests.Unit.Builders;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventRepositoryTest
    {
        private readonly EventRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly Mock<IEventCategoriesFactory> _eventCategoriesFactory = new Mock<IEventCategoriesFactory>();
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulAlert>, Alert>> _alertFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
        private readonly Mock<ILogger<EventRepository>> _logger;
        private readonly ICacheWrapper _cacheWrapper;

        public EventRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            var documentFactory = new DocumentContentfulFactory();
            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _alertFactory = new Mock<IContentfulFactory<Entry<ContentfulAlert>, Alert>>();
            _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
            _alertFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulAlert>>())).Returns(new Alert("title", "subHeading", "body",
                                                                 "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                                 new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)));

            _mockTimeProvider = new Mock<ITimeProvider>();
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            var contentfulFactory = new EventContentfulFactory(documentFactory, _groupFactory.Object, _alertFactory.Object, _mockTimeProvider.Object);
            _httpClient = new Mock<IHttpClient>();
            
            _logger = new Mock<ILogger<EventRepository>>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            //var memoryCache = new MemoryDistributedCache(new DistributedCacheEntryOptions());
            //_cacheWrapper = new CacheWrapper(memoryCache);

            _repository = new EventRepository(config, _httpClient.Object, contentfulClientManager.Object,
                _mockTimeProvider.Object, contentfulFactory, _eventCategoriesFactory.Object, null, _logger.Object);
        }

        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            const string slug = "event-of-the-century";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();

            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(2);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> {rawEvent});

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
                        .Excluding(e => e.Alerts));

            eventItem.ThumbnailImageUrl.Should().Be(rawEvent.Image.File.Url + "?h=250");
            eventItem.ImageUrl.Should().Be(rawEvent.Image.File.Url);
            eventItem.Documents.Count.Should().Be(rawEvent.Documents.Count);
            eventItem.BookingInformation.Should().Be(rawEvent.BookingInformation);
            eventItem.Alerts.Count.Should().Be(1);
            eventItem.Alerts[0].Title.Should().Be(rawEvent.Alerts[0].Fields.Title);
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
                new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01))
                    .Frequency(frequency)
                    .Occurrences(occurences)
                    .Build();

            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(2);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()),                                                                            It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 04, 02)));
            var eventItem = response.Get<Event>();

            eventItem.EventDate.Should().Be(new DateTime(2017, 04, 02));
        }

        [Fact]
        public void GetsA404ForANotFoundEventItem()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 08, 5));

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent>());

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
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0, null, null));

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
                    .Excluding(e => e.Group)
                    .Excluding(e => e.Alerts));

            eventCalender.Events.Last()
                .ShouldBeEquivalentTo(anotherEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl)
                    .Excluding(e => e.ImageUrl)
                    .Excluding(e => e.Documents)
                    .Excluding(e => e.UpdatedAt)
                    .Excluding(e => e.Group)
                    .Excluding(e => e.Alerts));
        }

        [Fact]
        public void ShouldGet404IfContentNotFound()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.IsAny<QueryBuilder<Event>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event>());

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null));

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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null));

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
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null));

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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response =
                AsyncTestHelper.Resolve(_repository.Get(new DateTime(2017, 04, 01), new DateTime(2017, 04, 16), null,  0,null, null));

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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  0,null, null));


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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 0,null, null));


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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  0,null, null));


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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo, null, 0,null, null));


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts));
        }

        [Fact]
        public void ShouldGetOneEventForACategory()
        {
            var anEvent =
                new ContentfulEventBuilder().EventCategory(new List<string> {"category 1", "category 2"}).Build();
            var anotherEvent = new ContentfulEventBuilder().EventCategory(new List<string> {"category 2"}).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent};


            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);


            var response = AsyncTestHelper.Resolve(_repository.Get(null, null,  "category 1", 0,null, null));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts));
        }

        [Fact]
        public void ShouldGetTwoEventsForACategory()
        {
            var anEvent =
                new ContentfulEventBuilder().EventCategory(new List<string> {"category 1", "category 2"}).Build();
            var anotherEvent = new ContentfulEventBuilder().EventCategory(new List<string> {"category 2"}).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent};

            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);


            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, "category 2", 0,null, null));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(2);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(anEvent,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts));
        }

        [Fact]
        public void ShouldGetEventForACategoryAndDate()
        {
            var dateFrom = new DateTime(2016, 07, 28);
            var dateTo = new DateTime(2017, 02, 15);

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));

            var event1 =
                new ContentfulEventBuilder().EventCategory(new List<string> {"category 1"})
                    .EventDate(new DateTime(2017, 08, 01))
                    .Build();
            var event2 =
                new ContentfulEventBuilder().EventCategory(new List<string> {"category 3"})
                    .EventDate(new DateTime(2016, 08, 01))
                    .Build();
            var event3 =
                new ContentfulEventBuilder().EventCategory(new List<string> {"category 1"})
                    .EventDate(new DateTime(2016, 08, 01))
                    .Build();
            var events = new List<ContentfulEvent> {event1, event2, event3};

            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo, "category 1", 0, null, null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First()
                .ShouldBeEquivalentTo(event3,
                    o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group).Excluding(e => e.Alerts));
        }

       
        public void ShouldGetEventsWithLimitOfTwo()
        {
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 09, 01)).Build();
            var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 10, 10)).Build();
            var eventThree = new ContentfulEventBuilder().EventDate(new DateTime(2017, 10, 11)).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent, eventThree};

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));

            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 2, null, null));

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
            _contentfulClient.Setup(
                o =>
                    o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(),
                        It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();
            eventItem.Featured.Should().BeTrue();
        }

        public void ShouldCreateEventWithFauteredSetToFalse()
        {
            const string slug = "event-of-the-century";
            var anEvent = new ContentfulEventBuilder().Featured(false).Build();
            _contentfulClient.Setup(
                o =>
                    o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(),
                        It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> { anEvent });

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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  2, true, null));

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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, 2, true, null));

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
            _contentfulClient.Setup(
                    o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null,  2, true, null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventsCalendar = response.Get<EventCalender>();

            eventsCalendar.Events[0].Featured.Should().BeFalse();
            eventsCalendar.Events[1].Featured.Should().BeFalse();
        }


        [Fact]
        public void ShouldGetLinkEventsForAGroup()
        {
            var anEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 01)).Build();
            var anotherEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 10, 10)).Build();
            var aThirdEvent = new ContentfulEventBuilder().Featured(false).EventDate(new DateTime(2017, 09, 15)).Build();
            var events = new List<ContentfulEvent> {anEvent, anotherEvent, aThirdEvent};


            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));

            _contentfulClient.Setup(
                   o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("slug"));

            response[0].Title.Should().Be("title");
            response[1].Description.Should().Be("description");

        }

        [Fact]
        public void ShouldReturnNoEventsIfTheGroupIsReferencedByNone()
        {
            // Arrange
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "slug").Include(2);

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulEvent>());

            // Act
            var events = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("slug"));

            // Assert
            events.Should().BeEmpty();
        }

        [Fact]
        public void ShouldReturnOneEventIfTheGroupIsReferencedByOne()
        {
            // Arrange
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "slug").Include(2);

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulEvent> { new ContentfulEventBuilder().Build() });

            _eventFactory.Setup(o => o.ToModel(It.IsAny<ContentfulEvent>())).Returns(new EventBuilder().Slug("slug").Build());
            // Act
            var events = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("slug"));

            // Assert
            events.Count.Should().Be(1);
            events[0].Slug.Should().Be("slug");
        }

        [Fact]
        public void ShouldReturnThreeEventsIfTheGroupIsReferencedByThree()
        {
            // Arrange
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "slug").Include(2);

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulEvent> { new ContentfulEventBuilder().Slug("event-slug").Build(), new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build() });

            _eventFactory.Setup(o => o.ToModel(It.IsAny<ContentfulEvent>())).Returns(new EventBuilder().Slug("event-slug").Build());
            // Act
            var events = AsyncTestHelper.Resolve(_repository.GetLinkedEvents<Group>("slug"));

            // Assert
            events.Count.Should().Be(3);
            events[0].Slug.Should().Be("event-slug");
        }

        //[Fact]
        //public void ShouldCallContentfulIfCacheIsEmpty()
        //{
        //    // Arrange
        //    var categories = new List<string> { "Arts and Crafts","Business Events","Sports","Museums","Charity","Council","Christmas","Dance","Education","Chadkirk Chapel",
        //                                        "Community Group","Public Health","Fayre","Talk","Environment","Comedy","Family","Armed Forces","Antiques and Collectors","Excercise and Fitness",
        //                                        "Fair","Emergency Services","Bonfire","Remembrence Service" };

        //    _eventCategoriesFactory.Setup(o => o.Build(It.IsAny<IList<dynamic>>())).Returns(categories);

        //    _cacheWrapper.RemoveItemFromCache("event-categories");

        //    // Act
        //    var response = _repository.GetCategories();

        //    // Assert
        //    _eventCategoriesFactory.Verify(x => x.Build(It.IsAny<IList<dynamic>>()), Times.Once);
        //}

        //[Fact]
        //public void ShouldNotCallContentfulIfCacheIsFull()
        //{
        //    // Arrange
        //    var categories = new List<string> { "Arts and Crafts","Business Events","Sports","Museums","Charity","Council","Christmas","Dance","Education","Chadkirk Chapel",
        //                                        "Community Group","Public Health","Fayre","Talk","Environment","Comedy","Family","Armed Forces","Antiques and Collectors","Excercise and Fitness",
        //                                        "Fair","Emergency Services","Bonfire","Remembrence Service" };

        //    _eventCategoriesFactory.Setup(o => o.Build(It.IsAny<IList<dynamic>>())).Returns(categories);

        //    _cacheWrapper.Set("event-categories", categories, new MemoryCacheEntryOptions());

        //    // Act
        //    var response = _repository.GetCategories();

        //    // Assert
        //    _eventCategoriesFactory.Verify(x => x.Build(It.IsAny<IList<dynamic>>()), Times.Never);
        //}
    }
}

