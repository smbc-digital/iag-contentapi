using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using HttpResponse = StockportContentApi.Http.HttpResponse;

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
        private readonly DateTime _eventDate = new DateTime(2016, 08, 03);

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
            var eventCalanderFactory = new Mock<IFactory<EventCalender>>();
            _repository = new EventRepository(config, _httpClient.Object, eventFactory.Object, eventCalanderFactory.Object, _mockTimeProvider.Object);

            eventFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(
                new Event(Title, Slug, Teaser, Image, ThumbnailImage, Description, _sunriseDate, _sunsetDate, Fee, Location, SubmittedBy, null, null, false, _eventDate, StartTime, EndTime, new List<Crumb>()));            
        }


        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 02));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1&fields.slug=event-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Event.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();

            eventItem.Title.Should().Be(Title);
            eventItem.Description.Should().Be(Description);
            eventItem.Slug.Should().Be(Slug);
            eventItem.Teaser.Should().Be(Teaser);
            eventItem.SunriseDate.Should().Be(_sunriseDate);
            eventItem.SunsetDate.Should().Be(_sunsetDate);
            eventItem.Image.Should().Be(Image);
            eventItem.Fee.Should().Be(Fee);
            eventItem.Location.Should().Be(Location);
            eventItem.SubmittedBy.Should().Be(SubmittedBy);
            eventItem.EventDate.Should().Be(_eventDate);
            eventItem.StartTime.Should().Be(StartTime);
            eventItem.EndTime.Should().Be(EndTime);
        }

        [Fact]
        public void GetsA404ForANotFoundEventItem()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1&fields.slug=event-not-found"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-not-found"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No event found for 'event-not-found'");
        }

        [Fact]
        public void GetsA404ForAEventOutsideOfSunriseDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 11, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1&fields.slug=event-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Event.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No event found for 'event-of-the-century'");
        }

        [Fact]
        public void GetsA404ForAEventOutsideOfSunsetDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 23));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1&fields.slug=event-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Event.json")));

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
        public void ShouldGetNoneEventsWithOutSideDates()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=events&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/EventsCalendar.json")));

            var response = AsyncTestHelper.Resolve(_repository.Get(null,null));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No events found");
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
