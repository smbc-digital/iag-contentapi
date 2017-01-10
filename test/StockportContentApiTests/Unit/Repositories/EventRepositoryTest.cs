using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventRepositoryTest
    {
        private readonly EventRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;

        public EventRepositoryTest()
        {
            var config = new ContentfulConfig("test")
               .Add("DELIVERY_URL", "https://fake.url")
               .Add("TEST_SPACE", "SPACE")
               .Add("TEST_ACCESS_KEY", "KEY")
               .Build();

            _mockTimeProvider = new Mock<ITimeProvider>();
            var eventFactory = new Mock<IFactory<Event>>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<Contentful.Core.IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _repository = new EventRepository(config, contentfulClientManager.Object, _mockTimeProvider.Object);

            eventFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(
                new Event("This is the event", "event-of-the-century", "Read more for the event", "image.jpg", "The event",
                          new DateTime(2016, 08, 01), new DateTime(2016, 08, 10), "Free", "Bramall Hall, Carpark, SK7 6HG", 
                          "Friends of Stockport", string.Empty, string.Empty, false, new DateTime(2016, 08, 08), "10:00", 
                          "17:00", 0, EventFrequency.None, new List<Crumb>()));            
        }

        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            const string slug = "event-of-the-century";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 07, 01), new DateTime(2017, 07, 01), 
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type")});
            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> {anEvent});

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();
            eventItem.Should().Be(anEvent);
        }

        [Fact]
        public void GetsParticularReccuringEventFromASlug()
        {
            const string slug = "event-of-the-century";
            const int occurences = 3;
            const EventFrequency frequency = EventFrequency.Daily;
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 07, 01), new DateTime(2017, 07, 01),
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 04, 02)));
            var eventItem = response.Get<Event>();

            eventItem.EventDate.Should().Be(new DateTime(2017, 04, 02));
        }

        [Fact]
        public void GetsA404ForANotFoundEventItem()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 08, 5));
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event>());

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-not-found", new DateTime(2017, 4, 1)));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No event found for 'event-not-found'");
        }

        [Fact]
        public void GetsA404ForAEventOutsideOfSunriseDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 11, 5));
            var sunriseDate = new DateTime(2016, 12, 01);
            var anEvent = new Event("title", "slug", "teaser", "image", "description", sunriseDate, new DateTime(2017, 07, 01),
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type") });
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century", new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No event found for 'event-of-the-century'");
        }

        [Fact]
        public void GetsA404ForAEventOutsideOfSunsetDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 6, 23));
            var sunsetDate = new DateTime(2017, 1, 01);
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), sunsetDate,
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type") });
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century", new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ShouldGetAllEvents()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var anotherEvent = new Event("title2", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> {anEvent, anotherEvent};
            var builder = new QueryBuilder().ContentTypeIs("events").Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null,null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().BeEquivalentTo(events);
        }

        [Fact]
        public void ShouldGet404IfContentNotFound()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event>());

            var response = AsyncTestHelper.Resolve(_repository.Get(null,null));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No events found");
        }

        [Fact]
        public void ShouldGetAllEventsWithTheirDailyReccuringParts()
        {
            const int occurences = 3;
            const EventFrequency frequency = EventFrequency.Daily;
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1),
                                   "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null));

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
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), 
                                   "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null));

            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Count.Should().Be(occurences);
            eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
            eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 04, 08));
            eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 04, 15));
            eventCalender.Events[3].EventDate.Should().Be(new DateTime(2017, 04, 22));
        }

        [Fact]
        public void ShouldGetWeeklyRepeatedDates_BetweenDates()
        {
            const int occurences = 4;
            const EventFrequency frequency = EventFrequency.Weekly;
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1),
                                   "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(new DateTime(2017, 04, 01), new DateTime(2017, 04, 16)));

            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Count.Should().Be(3);
            eventCalender.Events[1].EventDate.Should().Be(new DateTime(2017, 04, 08));
            eventCalender.Events[2].EventDate.Should().Be(new DateTime(2017, 04, 15));
        }

        [Fact]
        public void ShouldGetAllEventsWithTheirForthnightlyReccuringParts()
        {
            const int occurences = 3;
            const EventFrequency frequency = EventFrequency.Fortnightly;
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1),
                                   "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null));

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
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1),
                                   "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null));

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
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 1, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1),
                                   "18:00", "22:00", occurences, frequency, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null));

            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Count.Should().Be(occurences);
            eventCalender.Events[0].EventDate.Should().Be(new DateTime(2017, 04, 01));
            eventCalender.Events[1].EventDate.Should().Be(new DateTime(2018, 04, 01));
            eventCalender.Events[2].EventDate.Should().Be(new DateTime(2019, 04, 01));          
        }

        [Fact]
        public void ShouldGetEventsWithinDateRange()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
            var anEvent = new Event("title", "slug", "teaser", "image", "description", new DateTime(2016, 12, 01), new DateTime(2017, 07, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 1, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var anotherEvent = new Event("title2", "slug", "teaser", "image", "description", new DateTime(2016, 05, 01), new DateTime(2017, 06, 01),
                                   "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", 0, EventFrequency.None, new List<Crumb>() { new Crumb("title", "slug", "type") });
            var events = new List<Event> { anEvent, anotherEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(new DateTime(2016, 07, 28), new DateTime(2017, 02, 15)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First().Should().Be(anEvent);
        }
    }
}
