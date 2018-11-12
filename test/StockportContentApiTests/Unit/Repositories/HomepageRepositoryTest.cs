using System.Collections.Generic;
using System.Net;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using IContentfulClient = Contentful.Core.IContentfulClient;
using Contentful.Core.Search;
using System.Threading;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Repositories
{
    public class HomepageRepositoryTest
    {
        private readonly HomepageRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulHomepage, Homepage>> _homepageFactory;
        private readonly Mock<IContentfulClient> _client;

        public HomepageRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _homepageFactory = new Mock<IContentfulFactory<ContentfulHomepage, Homepage>>();

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
            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulHomepage>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _homepageFactory.Setup(o => o.ToModel(It.IsAny<ContentfulHomepage>()))
                .Returns(new Homepage(new List<string>(), string.Empty, string.Empty, new List<SubItem>(), new List<SubItem>(), new List<Alert>(), new List<CarouselContent>(), string.Empty, string.Empty, null, string.Empty));

            var response = AsyncTestHelper.Resolve(_repository.Get());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
