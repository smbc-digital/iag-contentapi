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
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class AlertListFactoryTest : TestingBaseClass
    {
        private readonly AlertListFactory _alertListFactory;

        public AlertListFactoryTest()
        {
            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 01));
            _alertListFactory = new AlertListFactory(mockTimeProvider.Object, new AlertFactory());
        }

        [Fact]
        public void BuildsAlertListFromContentfulResponseData()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Alert.ArticleWithOnlyAlerts.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();
            IEnumerable<Alert> alerts = _alertListFactory.BuildFromReferences(article.fields.alerts, contentfulResponse);

            alerts.Should().HaveCount(2);
            alerts.First().Title.Should().Be("Information alert");
            alerts.First().Body.Should().Be("This is an information alert.");
            alerts.First().SubHeading.Should().Be("test");
            alerts.First().Severity.Should().Be("Information");
        }

        [Fact]
        public void BuildEmptyAlertListForEmptyEntry()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Alert.ArticleWithOnlyAlerts.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var alert = _alertListFactory.BuildFromReferences(null, contentfulResponse);

            alert.Should().BeEmpty();
        }

        [Fact]
        public void BuildListOfAlertsThatHaveStartedButNotFinished()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Alert.ArticleWithListOfPastCurrentAndFutureAlerts.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();
            IEnumerable<Alert> alerts = _alertListFactory.BuildFromReferences(article.fields.alerts, contentfulResponse);

            alerts.Should().HaveCount(2);
            alerts.First().Title.Should().Be("Now Alert");
            alerts.Last().Title.Should().Be("Now 2 Alert");
        }
    }
}