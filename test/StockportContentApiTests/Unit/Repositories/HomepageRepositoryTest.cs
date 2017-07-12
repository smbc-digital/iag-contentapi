using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Utils;
using Microsoft.Extensions.Logging;
using IContentfulClient = Contentful.Core.IContentfulClient;
using Contentful.Core.Search;
using System.Threading;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Repositories
{
    public class HomepageRepositoryTest
    {
        private readonly HomepageRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulFactory<ContentfulHomepage, Homepage>> _homepageFactory;
        private readonly Mock<IContentfulFactory<List<ContentfulGroup>, List<Group>>> _listGroupFactory;
        private readonly Mock<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>> _listGroupCategoryFactory;
        private readonly Mock<IContentfulClient> _client;
        private readonly Mock<ITimeProvider> _timeProvider;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<ILogger<HomepageRepository>> _logger;

        public HomepageRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _homepageFactory = new Mock<IContentfulFactory<ContentfulHomepage, Homepage>>();
            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _timeProvider = new Mock<ITimeProvider>();
            _listGroupFactory = new Mock<IContentfulFactory<List<ContentfulGroup>, List<Group>>>();
            _listGroupCategoryFactory = new Mock<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>>();
            _logger = new Mock<ILogger<HomepageRepository>>();
            _cacheWrapper = new Mock<ICache>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _repository = new HomepageRepository(config, contentfulClientManager.Object, _homepageFactory.Object);
        }

        [Fact]
        public void ItGetsHomepage()
        {
            var contentfulHomepage = new ContentfulHomepage();
            var collection = new ContentfulCollection<ContentfulHomepage>();
            collection.Items = new List<ContentfulHomepage> { contentfulHomepage };

            var builder = new QueryBuilder<ContentfulHomepage>().ContentTypeIs("homepage").Include(2);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulHomepage>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _homepageFactory.Setup(o => o.ToModel(It.IsAny<ContentfulHomepage>()))
                .Returns(new Homepage(new List<string>(), string.Empty, string.Empty, new List<SubItem>(), new List<SubItem>(), new List<Alert>(), new List<CarouselContent>(), string.Empty, string.Empty, null));

            var response = AsyncTestHelper.Resolve(_repository.Get());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
