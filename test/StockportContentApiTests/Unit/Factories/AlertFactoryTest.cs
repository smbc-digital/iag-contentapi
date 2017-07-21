using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class AlertFactoryTest : TestingBaseClass
    {
        private readonly IFactory<Alert> _alertFactory;

        public AlertFactoryTest()
        {
            _alertFactory = new AlertFactory();
        }

        [Fact]
        public void BuildsAnAlert()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Alert.Alert.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Alert alert = _alertFactory.Build(entry, contentfulResponse);

            alert.Title.Should().Be("Information alert");
            alert.SubHeading.Should().Be("test");
            alert.Body.Should().Be("This is an information alert.");
            alert.Severity.Should().Be("Information");
            alert.SunriseDate.Should().Be(new DateTime(2016, 06, 30, 23, 0, 0, 0, DateTimeKind.Utc));
            alert.SunsetDate.Should().Be(new DateTime(2016, 08, 30, 23, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void BuildAnEmptyAlertIfNoContent()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Alert alert = _alertFactory.Build(entry, contentfulResponse);

            alert.Should().BeOfType<NullAlert>();
        }

    }
}
