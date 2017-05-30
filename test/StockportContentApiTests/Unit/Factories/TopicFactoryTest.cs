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
    public class TopicFactoryTest
    {
        private readonly TopicFactory _topicBuilder;
        private readonly Mock<IBuildContentTypesFromReferences<SubItem>> _mockSubitemBuilder;
        private IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;
        private readonly Mock<IBuildContentTypeFromReference<EventBanner>> _eventBannerFactory;

        private readonly EventBanner _eventBanner = new EventBanner("Title", "teaser", "icon", "link");

        public TopicFactoryTest()
        {
            _mockSubitemBuilder = new Mock<IBuildContentTypesFromReferences<SubItem>>();
            _eventBannerFactory =  new Mock<IBuildContentTypeFromReference<EventBanner>>();
            var mockAlertListBuilder = new Mock<IBuildContentTypesFromReferences<Alert>>();
            mockAlertListBuilder.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))

                .Returns(new List<Alert>());
            _mockSubitemBuilder.Setup(
                   o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
               .Returns(new List<SubItem> { new SubItem("slug", "title", "teaser", "ison", string.Empty,DateTime.MinValue, DateTime.MinValue, "image", new List<SubItem>()) });

            _eventBannerFactory.Setup(
                    o => o.BuildFromReference(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(_eventBanner);

            _breadcrumbFactory = new BreadcrumbFactory();
            _topicBuilder = new TopicFactory(mockAlertListBuilder.Object, _mockSubitemBuilder.Object, _breadcrumbFactory, _eventBannerFactory.Object);
        }

        [Fact]
        public void BuildTopicWithBreadcrumbs()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ListOfArticlesForTopic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Topic topic = _topicBuilder.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            topic.Breadcrumbs.Should().HaveCount(1);
            topic.Breadcrumbs.First().Title.Should().Be("Healthy Living");
            topic.Breadcrumbs.First().Slug.Should().Be("healthy-living");
        }

        [Fact]
        public void BuildTopicBySlugThatIncludesPrimarySecondaryAndTertiaryItems()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/TopicWithPrimarySecondaryAndTertiaryItems.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Topic topic = _topicBuilder.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            _mockSubitemBuilder.Verify(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()), Times.AtLeastOnce);

            topic.Name.Should().Be("Test topic With Subtopic");
            topic.Slug.Should().Be("test-topic-with-subtopic");
            topic.EmailAlerts.Should().Be(true);
            topic.EmailAlertsTopicId.Should().Be("test-id");
        }

        [Fact]
        public void BuildTopicWithoutSummaryOrBackgroundImage()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/TopicWithoutSummary.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Topic topic = _topicBuilder.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            topic.Name.Should().Be("Healthy Living");
            topic.Slug.Should().Be("healthy-living");
        }

        [Fact]
        public void BuildTopic()
        {
            var mockListAlertBuilder = new Mock<IBuildContentTypesFromReferences<Alert>>();
            var alerts = new List<Alert>() { new NullAlert()};
            mockListAlertBuilder.Setup(
                o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(alerts);
            var topicBuilder = new TopicFactory(mockListAlertBuilder.Object, _mockSubitemBuilder.Object, _breadcrumbFactory, _eventBannerFactory.Object);

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Topic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            Topic topic = topicBuilder.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            topic.Name.Should().Be("Main Topic");
            topic.Slug.Should().Be("main-topic");
            topic.Teaser.Should().Be("This is the main topic");
            topic.Summary.Should().Be("This is the summary");
            topic.Icon.Should().Be("si-car");
            topic.BackgroundImage.Should().Be(string.Empty);
            topic.Image.Should().Be(string.Empty);
            topic.Breadcrumbs.ToList().Count.Should().Be(1);
            topic.Alerts.ToList().Count.Should().Be(1);
            topic.SubItems.Should().HaveCount(1);
            topic.TertiaryItems.Should().HaveCount(1);
            topic.SecondaryItems.Should().HaveCount(1);
            topic.EmailAlerts.Should().Be(true);
            topic.EmailAlertsTopicId.Should().Be("test-id");

            _mockSubitemBuilder.Verify(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()), Times.AtLeastOnce);
        }
    }
}
