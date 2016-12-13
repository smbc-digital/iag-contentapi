using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class EventFactoryTest
    {
        private readonly IFactory<Event> _eventFactory;

        public EventFactoryTest()
        {
            _eventFactory = new EventFactory();                  
        }

        [Fact]
        public void BuildsEvent()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Event.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Event eventItem = _eventFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            eventItem.Title.Should().Be("This is the event");
            eventItem.Slug.Should().Be("event-of-the-century");
            eventItem.Teaser.Should().Be("Read more for the event");           
            eventItem.Description.Should().Be("The event  description");
            eventItem.SunriseDate.Should().Be(new DateTime(2016, 12, 08, 00, 00, 00));
            eventItem.SunsetDate.Should().Be(new DateTime(2016, 12, 22, 00, 00, 00));
            eventItem.Fee.Should().Be("Free");
            eventItem.Location.Should().Be("Bramall Hall, Carpark, SK7 6HG");
            eventItem.SubmittedBy.Should().Be("Friends of Stockport");
            eventItem.EventDate.Should().Be(new DateTime(2016, 12, 30, 00, 00, 00));
            eventItem.StartTime.Should().Be("10:00");
            eventItem.EndTime.Should().Be("17:00");
        }

        [Fact]
        public void BuildsEventWithImage()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/EventWithImage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Event eventItem = _eventFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            eventItem.Title.Should().Be("This is the event");
            eventItem.Slug.Should().Be("event-of-the-century");
            eventItem.Teaser.Should().Be("Read more for the event");
            eventItem.Description.Should().Be("The event  description");
            eventItem.Image.Should().Be("image.jpg");
            eventItem.ThumbnailImage.Should().Be("image.jpg?h=250");
            eventItem.SunriseDate.Should().Be(new DateTime(2016, 12, 08, 00, 00, 00));
            eventItem.SunsetDate.Should().Be(new DateTime(2016, 12, 22, 00, 00, 00));
            eventItem.Fee.Should().Be("Free");
            eventItem.Location.Should().Be("Bramall Hall, Carpark, SK7 6HG");
            eventItem.SubmittedBy.Should().Be("Friends of Stockport");
            eventItem.EventDate.Should().Be(new DateTime(2016, 12, 30, 00, 00, 00));
            eventItem.StartTime.Should().Be("10:00");
            eventItem.EndTime.Should().Be("17:00");
        }
    }
}
