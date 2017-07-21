using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;
using System.Linq;
using FluentAssertions;

namespace StockportContentApiTests.Unit.Factories
{
    public class NewsroomFactoryTest : TestingBaseClass
    {
        private readonly IFactory<Newsroom> _newsroomFactory;

        public NewsroomFactoryTest()
        {
            var mockAlertListFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            var alert = new Alert(_alertTitle, _alertSubHeading, _alertBody, _alertSeverity, _alertSunriseDate, _alertSunsetDate);
            mockAlertListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Alert>() { alert });
            _newsroomFactory = new NewsroomFactory(mockAlertListFactory.Object);
        }

        private readonly string _alertTitle = "Title";
        private readonly string _alertSubHeading = "SubHeading";
        private readonly string _alertBody = "Body";
        private readonly string _alertSeverity = "Error";
        private readonly DateTime _alertSunriseDate = new DateTime(2016, 10, 10);
        private readonly DateTime _alertSunsetDate = new DateTime(2016, 10, 20);

        [Fact]
        public void BuildsNewsroom()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Newsroom.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Newsroom newsroom = _newsroomFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            newsroom.Alerts.Count.Should().Be(1);
            var alert = newsroom.Alerts.First();
            alert.Title.Should().Be(_alertTitle);
            alert.SubHeading.Should().Be(_alertSubHeading);
            alert.Body.Should().Be(_alertBody);
            alert.Severity.Should().Be(_alertSeverity);
            alert.SunriseDate.Should().Be(_alertSunriseDate);
            alert.SunsetDate.Should().Be(_alertSunsetDate);

            newsroom.EmailAlerts.Should().Be(true);
            newsroom.EmailAlertsTopicId.Should().Be("test-id");
        }
    }
}
