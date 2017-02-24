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
    public class HomepageFactoryTest
    {
        private IFactory<Homepage> _homepageFactory;
        private readonly Mock<IBuildContentTypesFromReferences<Topic>> _mockTopicListBuilder;
        private readonly Mock<IBuildContentTypesFromReferences<Alert>> _mockAlertListFactory;
        private readonly Mock<IBuildContentTypesFromReferences<CarouselContent>> _mockCarouselContentListFactory;
        private readonly Mock<IBuildContentTypesFromReferences<SubItem>> _mockSubitemListFactory;
        private readonly Mock<ITimeProvider> _mockTimeProvider;

        public HomepageFactoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 01));

            _mockTopicListBuilder = new Mock<IBuildContentTypesFromReferences<Topic>>();
            _mockTopicListBuilder.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<object>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Topic>() {new NullTopic(), new NullTopic()});

            _mockAlertListFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            _mockAlertListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Alert>());

            _mockCarouselContentListFactory = new Mock<IBuildContentTypesFromReferences<CarouselContent>>();
            _mockCarouselContentListFactory.Setup(
                o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                    .Returns(new List<CarouselContent>());

            _mockSubitemListFactory = new Mock<IBuildContentTypesFromReferences<SubItem>>();
            _mockSubitemListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<SubItem> { new SubItem("slug", "title", "teaser", "ison", string.Empty,DateTime.MinValue, DateTime.MinValue, "image") });

            _homepageFactory = new HomepageFactory(_mockTopicListBuilder.Object, _mockAlertListFactory.Object, _mockCarouselContentListFactory.Object, _mockSubitemListFactory.Object);
        }

        [Fact]
        public void BuildHomepage()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            var homepage = (Homepage)_homepageFactory.Build(entry, contentfulResponse);

            homepage.FeaturedTasksHeading.Should().Be("Featured tasks heading");
            homepage.FeaturedTasksSummary.Should().Be("Featured tasks summary");
            homepage.FreeText.Should().Be("homepage text");


            _mockSubitemListFactory.Verify(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()), Times.AtLeastOnce);
        }

        [Fact]
        public void BuildHomepageWithPopularSearchTerms()
        {
            dynamic mockContentfulData =
                 JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            var homepage = (Homepage)_homepageFactory.Build(entry, contentfulResponse);

            homepage.PopularSearchTerms.Should().NotBeEmpty();
            homepage.PopularSearchTerms.First().Should().Be("popular search term");
        }

        [Fact]
        public void BuildHomepageWithNoTopicsAndTasksAndAlerts()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/HomepageWithNoTopicsAndTasks.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            var homepage = (Homepage)_homepageFactory.Build(entry, contentfulResponse);

            homepage.Alerts.Should().HaveCount(0);
            homepage.PopularSearchTerms.Should().BeEmpty();
        }

        [Fact]
        public void BuildHomepageWithCarousel()
        {
            var carouselContents = new CarouselContent("Red Rock",
                "red-rock",
                "The long awaited cinema complex is due to open late Oct 2016. Come and take a look.",
                "image.jpg",
                new DateTime(), new DateTime(), "http://fake.url");
            _mockCarouselContentListFactory.Setup(
                   o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<CarouselContent>() { carouselContents });

            _homepageFactory = new HomepageFactory(_mockTopicListBuilder.Object, _mockAlertListFactory.Object, _mockCarouselContentListFactory.Object, _mockSubitemListFactory.Object);

            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            var homepage = (Homepage)_homepageFactory.Build(entry, contentfulResponse);

            //Carousel Content tests
            homepage.CarouselContents.Should().NotBeNullOrEmpty();
            homepage.CarouselContents.First().Title.Should().Contain(carouselContents.Title);
            homepage.CarouselContents.First()
                .Teaser.Should()
                .Contain(carouselContents.Teaser);
            homepage.CarouselContents.First().Url.Should().Contain(carouselContents.Url);
            homepage.CarouselContents.First()
                .Image.Should()
                .Contain(carouselContents.Image);

            _mockSubitemListFactory.Verify(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()), Times.AtLeastOnce);
        }

        [Fact]
        public void BuildHomepageWithFeaturedTopics()
        {
            var topic = new NullTopic();
            _mockTopicListBuilder.Setup(
                   o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Topic>() { topic });

            _homepageFactory = new HomepageFactory(_mockTopicListBuilder.Object, _mockAlertListFactory.Object, _mockCarouselContentListFactory.Object, _mockSubitemListFactory.Object);

            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Homepage homepage = _homepageFactory.Build(entry, contentfulResponse);

            homepage.FeaturedTopics.Should().HaveCount(1);
            homepage.FeaturedTasks.Should().HaveCount(1);

            _mockSubitemListFactory.Verify(
                o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()),
                Times.AtLeastOnce());
        }

        public void BuildHomepageWithAlerts()
        {
            var alert = new Alert("Alert", "alert", "alert message", "Information", new DateTime(2000, 08, 02), new DateTime(2216, 08, 01));
            var mockAlertBuilder = new Mock<IFactory<Alert>>();
            mockAlertBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(alert);

            _homepageFactory = new HomepageFactory(_mockTopicListBuilder.Object, new AlertListFactory(_mockTimeProvider.Object, mockAlertBuilder.Object), _mockCarouselContentListFactory.Object, _mockSubitemListFactory.Object);

            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Homepage homepage = _homepageFactory.Build(entry, contentfulResponse);

            homepage.Alerts.Count().Should().Be(3);
        }
    }
}
