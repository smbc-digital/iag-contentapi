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
    public class TopicListFactoryTest
    {
        private readonly TopicListFactory _topicListFactory;
        private Mock<ITimeProvider> _mockTimeProvider;
        private readonly ExpandingLinkBox _expandingLinkBox = new ExpandingLinkBox("Title", null);

        public TopicListFactoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            var mockSubitemBuilder = new Mock<IBuildContentTypesFromReferences<SubItem>>();
            var mockListAlertBuilder = new Mock<IBuildContentTypesFromReferences<Alert>>();
            var mockEventBannerBuilder = new Mock<IBuildContentTypeFromReference<EventBanner>>();
            var mockExpandingLinkBoxBuilder = new Mock<IBuildContentTypesFromReferences<ExpandingLinkBox>>();            

        mockListAlertBuilder.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))

                .Returns(new List<Alert>());
            mockSubitemBuilder.Setup(
                   o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
               .Returns(new List<SubItem> { new SubItem("slug", "title", "teaser", "ison", string.Empty,DateTime.MinValue, DateTime.MinValue, "image", new List<SubItem>()) });

            mockEventBannerBuilder.Setup(
                    o => o.BuildFromReference(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new EventBanner("title", "teaser", "icon", "link"));

            mockExpandingLinkBoxBuilder.Setup(
                     o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
               .Returns(new List<ExpandingLinkBox> { _expandingLinkBox });

            var breadcrumbFactory = new BreadcrumbFactory();
            _topicListFactory = new TopicListFactory(new TopicFactory(mockListAlertBuilder.Object, mockSubitemBuilder.Object, breadcrumbFactory,mockEventBannerBuilder.Object, mockExpandingLinkBoxBuilder.Object), _mockTimeProvider.Object);
        }


        [Fact]
        public void BuildTopicListFromReferences()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage/HomepageWithOnlyFeaturedTopics.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);
            var homepage = contentfulResponse.GetFirstItem();

            List<Topic> topics = _topicListFactory.BuildFromReferences(homepage.fields.featuredTopics, contentfulResponse);

            topics.Should().HaveCount(2);
            topics.First().Slug.Should().Be("main-topic");
            topics.Last().Slug.Should().Be("main-topic");
        }

        [Fact]
        public void BuildEmptyTopicListWhenEntryIsEmpty()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage/HomepageWithOnlyFeaturedTopics.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            List<Topic> topics = _topicListFactory.BuildFromReferences(null, contentfulResponse).ToList();

            topics.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsCorectSizeTopicListForSunriseSunsetDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2018, 01, 15));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Homepage/HomepageWithOnlyFeaturedTopicsWithDate.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);
            var homepage = contentfulResponse.GetFirstItem();

            List<Topic> topics = _topicListFactory.BuildFromReferences(homepage.fields.featuredTopics,
                contentfulResponse);

            topics.Should().HaveCount(1);
        }

        
    }
}
