using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public class SubItemListFactoryTest : TestingBaseClass
    {
        private readonly SubItemListFactory _factory;
        private dynamic _topicItem;
        private  ContentfulResponse _topicContentfulResponse;
        private Mock<ITimeProvider> _mockTimeProvider;

        public SubItemListFactoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            _factory = new SubItemListFactory(new SubItemFactory(), _mockTimeProvider.Object);
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.TopicWithPrimarySecondaryAndTertiaryItems.json"));
            _topicContentfulResponse = new ContentfulResponse(mockContentfulData);
            _topicItem = _topicContentfulResponse.GetFirstItem();
        }

        [Fact]
        public void BuildsListOfTopicSubItems()
        {
            IEnumerable<dynamic> subItems = (IEnumerable<dynamic>)_topicItem.fields.subItems;

            IEnumerable<SubItem> items = _factory.BuildFromReferences(subItems, _topicContentfulResponse);

            items.Should().HaveCount(3);
            items.First().Title.Should().Be("Test Video ");
            items.First().Slug.Should().Be("test-video");
        }

        [Fact]
        public void BuildsListOfTopicSecondaryItems()
        {
            IEnumerable<dynamic> subItems = (IEnumerable<dynamic>)_topicItem.fields.secondaryItems;

            IEnumerable<SubItem> items = _factory.BuildFromReferences(subItems, _topicContentfulResponse);

            items.Should().HaveCount(1);
            items.First().Title.Should().Be("Test sub topic 1");
            items.First().Slug.Should().Be("test-sub-topic-1");
        }

        [Fact]
        public void BuildsListOfTopicTertiaryItems()
        {
            IEnumerable<dynamic> subItems = (IEnumerable<dynamic>)_topicItem.fields.tertiaryItems;

            IEnumerable<SubItem> items = _factory.BuildFromReferences(subItems, _topicContentfulResponse);

            items.Should().HaveCount(1);
            items.First().Title.Should().Be("Article with picture background");
            items.First().Slug.Should().Be("article-with-picture-background");
        }

        [Fact]
        public void DoesntBuildAnythingForEmptyList()
        {
            var items = _factory.BuildFromReferences(new List<dynamic>(), _topicContentfulResponse);
            items.Count().Should().Be(0);
        }

        [Fact]
        public void ReturnsCorectSizeSubItemListForSunriseSunsetDate()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.TopicWithPrimarySecondaryAndTertiaryItemsWithDate.json"));
            _topicContentfulResponse = new ContentfulResponse(mockContentfulData);
            _topicItem = _topicContentfulResponse.GetFirstItem();

            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            IEnumerable<dynamic> subItems = (IEnumerable<dynamic>) _topicItem.fields.subItems;

            IEnumerable<SubItem> items = _factory.BuildFromReferences(subItems, _topicContentfulResponse);

            items.Count().Should().Be(2);
            items.First().Title.Should().Be("Article with picture background");
            items.Last().Title.Should().Be("Test sub topic 1");
        }
    }
}
