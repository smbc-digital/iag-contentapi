using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
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
        }
    }
}
