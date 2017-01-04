using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using FluentAssertions;
using FluentAssertions.Common;
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
using HttpResponse = StockportContentApi.Http.HttpResponse;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly EventRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private const string Title = "This is the event";
        private const string Description = "The event";
        private const string Slug = "event-of-the-century";
        private const string Teaser = "Read more for the event";
        private readonly DateTime _sunriseDate = new DateTime(2016, 08, 01);
        private readonly DateTime _sunsetDate = new DateTime(2016, 08, 10);
        private const string Image = "image.jpg";
        private const string ThumbnailImage = "thumbnail.jpg";
        private const string Fee = "Free";
        private const string Location = "Bramall Hall, Carpark, SK7 6HG";
        private const string SubmittedBy = "Friends of Stockport";
        private const string StartTime = "10:00";
        private const string EndTime = "17:00";
        private readonly DateTime _eventDate = new DateTime(2016, 08, 08);
        private readonly Mock<IContentfulClient> _contentfulClient;

        public EventRepositoryTest()
        {
            var config = new ContentfulConfig("test")
               .Add("DELIVERY_URL", "https://fake.url")
               .Add("TEST_SPACE", "SPACE")
               .Add("TEST_ACCESS_KEY", "KEY")
               .Build();

            _mockTimeProvider = new Mock<ITimeProvider>();
            _httpClient = new Mock<IHttpClient>();            
            var eventFactory = new Mock<IFactory<Event>>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<Contentful.Core.IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _repository = new EventRepository(config, _httpClient.Object, contentfulClientManager.Object, eventFactory.Object, _mockTimeProvider.Object);

            eventFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(
                new Event(Title, Slug, Teaser, Image, ThumbnailImage, Description, _sunriseDate, _sunsetDate, Fee, Location, SubmittedBy, null, null, false, _eventDate, StartTime, EndTime, new List<Crumb>()));            
        }

        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            const string slug = "event-of-the-century";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));
            var anEvent = new Event("title", "slug", "teaser", "image", "thumbnailImage", "description", new DateTime(2016, 07, 01), new DateTime(2017, 07, 01), 
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", new List<Crumb>() { new Crumb("title", "slug", "type")});
            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> {anEvent});

            var response = AsyncTestHelper.Resolve(_repository.GetEvent(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();
            eventItem.Should().Be(anEvent);
        }

        [Fact]
        public void GetsA404ForANotFoundEventItem()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 08, 5));
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event>());

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-not-found"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No event found for 'event-not-found'");
        }

        [Fact]
        public void GetsA404ForAEventOutsideOfSunriseDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 11, 5));
            var sunriseDate = new DateTime(2016, 12, 01);
            var anEvent = new Event("title", "slug", "teaser", "image", "thumbnailImage", "description", sunriseDate, new DateTime(2017, 07, 01),
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", new List<Crumb>() { new Crumb("title", "slug", "type") });
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No event found for 'event-of-the-century'");
        }

        [Fact]
        public void GetsA404ForAEventOutsideOfSunsetDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 6, 23));
            var sunsetDate = new DateTime(2017, 1, 01);
            var anEvent = new Event("title", "slug", "teaser", "image", "thumbnailImage", "description", new DateTime(2016, 12, 01), sunsetDate,
                                    "fee", "location", "submittedBy", "longitude", "latitude", true, new DateTime(2017, 4, 1), "18:00", "22:00", new List<Crumb>() { new Crumb("title", "slug", "type") });
            _contentfulClient.Setup(o => o.GetEntriesAsync<Event>(It.IsAny<QueryBuilder>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> { anEvent });

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void GetAllEvents()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/EventsCalendar.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(null,null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var events = response.Get<EventCalender>();

            events.Events.Should().HaveCount(3);
            events.Events[0].Title.Should().Be(Title);
            events.Events[0].Slug.Should().Be(Slug);
            events.Events[0].Description.Should().Be(Description);
            events.Events[0].Teaser.Should().Be(Teaser);
            events.Events[0].SunriseDate.Should().Be(_sunriseDate);
            events.Events[0].SunsetDate.Should().Be(_sunsetDate);
            events.Events[0].Fee.Should().Be(Fee);
            events.Events[0].Location.Should().Be(Location);
            events.Events[0].SubmittedBy.Should().Be(SubmittedBy);
            events.Events[0].EventDate.Should().Be(_eventDate);
            events.Events[0].StartTime.Should().Be(StartTime);
            events.Events[0].EndTime.Should().Be(EndTime);
        }

        [Fact]
        public void ShouldGet404IfContentNotFound()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(null,null));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No events found");
        }

        [Fact]
        public void ShouldGetEventsWithinDateRange()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 08));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/EventsCalendar.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(new DateTime(2016, 07, 28), new DateTime(2016, 08, 30)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var events = response.Get<EventCalender>();
            events.Events.Should().HaveCount(3);
        }
    }
}
