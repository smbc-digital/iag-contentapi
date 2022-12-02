using System.Net;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventCategoryRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly EventCategoryRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _contentfulEventCategoryFactory;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<IConfiguration> _configuration;
        public EventCategoryRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            _contentfulEventCategoryFactory = new Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _cacheWrapper = new Mock<ICache>();
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");
            _repository = new EventCategoryRepository(config, _contentfulEventCategoryFactory.Object, contentfulClientManager.Object, _cacheWrapper.Object, _configuration.Object);
        }

        [Fact]
        public void ItGetsEventCategories()
        {
            // Arrange
            var rawEventCategory = new EventCategory("name", "slug", "icon");
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-categories-content-type"), It.IsAny<Func<Task<List<EventCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<EventCategory> { rawEventCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetEventCategories());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ShouldReturnNotFoundIfNoEventCategoryFound()
        {
            var collection = new ContentfulCollection<ContentfulEventCategory>();
            collection.Items = new List<ContentfulEventCategory>();

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-categories-content-type"), It.IsAny<Func<Task<List<EventCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<EventCategory>());

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetEventCategories());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
