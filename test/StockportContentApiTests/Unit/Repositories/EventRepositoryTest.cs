﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
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

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventRepositoryTest
    {
        private readonly EventRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly Mock<IEventCategoriesFactory> _eventCategoriesFactory = new Mock<IEventCategoriesFactory>();

        public EventRepositoryTest()
        {
            var config = new ContentfulConfig("test")
               .Add("DELIVERY_URL", "https://fake.url")
               .Add("TEST_SPACE", "SPACE")
               .Add("TEST_ACCESS_KEY", "KEY")
               .Build();

            var contentfulFactory = new EventContentfulFactory();
            _httpClient = new Mock<IHttpClient>();
            _mockTimeProvider = new Mock<ITimeProvider>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _repository = new EventRepository(config, _httpClient.Object, contentfulClientManager.Object, _mockTimeProvider.Object, contentfulFactory,_eventCategoriesFactory.Object);
        }

        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            const string slug = "event-of-the-century";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> {rawEvent});

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 4, 1)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();

            eventItem.ShouldBeEquivalentTo(rawEvent, o => o.Excluding(raw => raw.ThumbnailImageUrl).Excluding(raw => raw.ImageUrl).Excluding(raw => raw.Documents));
            eventItem.ThumbnailImageUrl.Should().Be(rawEvent.Image.File.Url + "?h=250");
            eventItem.ImageUrl.Should().Be(rawEvent.Image.File.Url);
            eventItem.Documents.Count.Should().Be(rawEvent.Documents.Count);
        }

        [Fact]
        public void GetsParticularReccuringEventFromASlug()
        {
            const string slug = "event-of-the-century";
            const int occurences = 3;
            const EventFrequency frequency = EventFrequency.Daily;
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));
            
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();

            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), 
                                                                            It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug, new DateTime(2017, 04, 02)));
            var eventItem = response.Get<Event>();

            eventItem.EventDate.Should().Be(new DateTime(2017, 04, 02));
        }

        [Fact]
        public void GetsA404ForANotFoundEventItem()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 08, 5));
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent>());

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
            var builder = new QueryBuilder().ContentTypeIs("events").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null,null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Count.Should().Be(2);
            eventCalender.Events.First().ShouldBeEquivalentTo(anEvent, o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents));
            eventCalender.Events.Last().ShouldBeEquivalentTo(anotherEvent, o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents));
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
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();
            var events = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

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
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();

            var events = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null));

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
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();
            var events = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response = AsyncTestHelper.Resolve(_repository.Get(new DateTime(2017, 04, 01), new DateTime(2017, 04, 16)));

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
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();
            var events = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

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
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();
            var events = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

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
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Frequency(frequency).Occurrences(occurences).Build();
            var events = new List<ContentfulEvent> { anEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

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
            var dateFrom = new DateTime(2016, 07, 28);
            var dateTo = new DateTime(2017, 02, 15);

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 08));
            var anEvent = new ContentfulEventBuilder().EventDate(new DateTime(2016, 09, 01)).Build();
            var anotherEvent = new ContentfulEventBuilder().EventDate(new DateTime(2017, 04, 01)).Build();
            var events = new List<ContentfulEvent> { anEvent, anotherEvent };
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);
           
            var response = AsyncTestHelper.Resolve(_repository.Get(dateFrom, dateTo));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventCalender = response.Get<EventCalender>();
            eventCalender.Events.Should().HaveCount(1);
            eventCalender.Events.First().ShouldBeEquivalentTo(anEvent, o => o.Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.ImageUrl).Excluding(e => e.Documents));
        }
    }
}
