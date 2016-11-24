using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class StartPageFactoryTest
    {
        private readonly IFactory<StartPage> _startPageFactory;
        private readonly Mock<IBuildContentTypesFromReferences<Alert>> _mockAlertListFactory;
        private readonly List<Alert> _alerts = new List<Alert>() { new Alert("title", "subHeading", "body", "severity", new DateTime(), new DateTime()) };

        public StartPageFactoryTest()
        {
            _mockAlertListFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            _mockAlertListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(_alerts);
            _startPageFactory = new StartPageFactory(new BreadcrumbFactory(), _mockAlertListFactory.Object);
        }

        [Fact]
        public void BuildStartPage()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/StartPage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            StartPage startPage = _startPageFactory.Build(entry, contentfulResponse);

            startPage.Title.Should().Be("Start Page");
            startPage.Slug.Should().Be("start-page");
            startPage.Teaser.Should().Be("this is a teaser");
            startPage.Summary.Should().Be("This is a summary ");
            startPage.UpperBody.Should().Be("An upper body");
            startPage.FormLink.Should().Be("http://start.com");
            startPage.FormLinkLabel.Should().Be("Start now");
            startPage.LowerBody.Should().Be("Lower body");
            startPage.BackgroundImage.Should().Be("image.jpg");
            startPage.Icon.Should().Be("icon");
            startPage.Breadcrumbs.Should().HaveCount(1);

            startPage.Alerts.Should().BeEquivalentTo(_alerts);
        }

        [Fact]
        public void ReturnNullStartPageIfItemListIsEmpty()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            StartPage startPage = _startPageFactory.Build(entry, contentfulResponse);

            startPage.Should().BeOfType<NullStartPage>();
        }
    }
}
