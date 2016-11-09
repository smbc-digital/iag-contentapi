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
    public class AlertFactoryTest
    {
        private readonly IFactory<Alert> _alertFactory;

        public AlertFactoryTest()
        {
            _alertFactory = new AlertFactory();
        }

        [Fact]
        public void BuildsAnAlert()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Alert/Alert.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Alert alert = _alertFactory.Build(entry, contentfulResponse);

            alert.Title.Should().Be("Information alert");
            alert.SubHeading.Should().Be("test");
            alert.Body.Should().Be("This is an information alert.");
            alert.Severity.Should().Be("Information");
            alert.SunriseDate.Should().Be(DateTime.Parse("2016-07-01T00:00+01:00"));
            alert.SunsetDate.Should().Be(DateTime.Parse("2016-08-31T00:00+01:00"));
        }

        [Fact]
        public void BuildAnEmptyAlertIfNoContent()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Alert alert = _alertFactory.Build(entry, contentfulResponse);

            alert.Should().BeOfType<NullAlert>();
        }

    }
}
