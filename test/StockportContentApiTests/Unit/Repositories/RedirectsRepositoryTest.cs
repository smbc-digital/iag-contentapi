using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using IContentfulClient = Contentful.Core.IContentfulClient;
using Contentful.Core.Models;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.Repositories
{
    public class RedirectsRepositoryTest
    {
        private readonly ContentfulConfig _config;
        private readonly Mock<Func<string, ContentfulConfig>> _createConfig;
        private readonly Mock<IContentfulClientManager> _contentfulClientManager;
        private readonly Mock<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>> _contenfulFactory;
        private readonly Mock<IContentfulClient> _client;
        private readonly ShortUrlRedirects _shortUrlRedirects = new ShortUrlRedirects(new Dictionary<string, RedirectDictionary>());
        private readonly LegacyUrlRedirects _legacyUrlRedirects = new LegacyUrlRedirects(new Dictionary<string, RedirectDictionary>());

        public RedirectsRepositoryTest()
        {
            _config = new ContentfulConfig("unittest")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("UNITTEST_SPACE", "SPACE")
                .Add("UNITTEST_ACCESS_KEY", "KEY")
                .Add("UNITTEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _createConfig = new Mock<Func<string, ContentfulConfig>>();
            _contenfulFactory = new Mock<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>>();
            _contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            _createConfig.Setup(o => o(It.IsAny<string>())).Returns(_config);
            _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
        }

        [Fact]
        public void ItGetsListOfRedirectsBack()
        {
            var ContentfulRedirects = new ContentfulRedirectBuilder().Build();
            var collection = new ContentfulCollection<ContentfulRedirect>();
            collection.Items = new List<ContentfulRedirect> { ContentfulRedirects };

            var redirectItem = new BusinessIdToRedirects(new Dictionary<string, string> { { "a-url", "another-url" } }, new Dictionary<string, string> { { "some-url", "another-url" } });

            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);
            _contenfulFactory.Setup(o => o.ToModel(ContentfulRedirects)).Returns(redirectItem);

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            var redirects = response.Get<Redirects>();

            var shortUrls = redirects.ShortUrlRedirects;
            shortUrls.Count.Should().Be(1);
            shortUrls.Keys.First().Should().Be("unittest");
            shortUrls["unittest"].ContainsKey("a-url").Should().BeTrue();
            var legacyUrls = redirects.LegacyUrlRedirects;
            legacyUrls.Count.Should().Be(1);
            legacyUrls.Keys.First().Should().Be("unittest");
            legacyUrls["unittest"].ContainsKey("some-url").Should().BeTrue();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ItGetsAnEmptyListForBusinessIdIfNoRedirectsFound()
        {
            var ContentfulRedirects = new ContentfulRedirect();
            var collection = new ContentfulCollection<ContentfulRedirect>();
            collection.Items = new List<ContentfulRedirect>();

            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);
            _contenfulFactory.Setup(o => o.ToModel(ContentfulRedirects)).Returns(new NullBusinessIdToRedirects());

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            var redirects = response.Get<Redirects>();

            var shortUrls = redirects.ShortUrlRedirects;
            shortUrls.Count.Should().Be(1);
            shortUrls["unittest"].Count.Should().Be(0);
            var legacyUrls = redirects.LegacyUrlRedirects;
            legacyUrls.Count.Should().Be(1);
            legacyUrls["unittest"].Count.Should().Be(0);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact]
        public void ItGets404BackForRedirects()
        {
            var collection = new ContentfulCollection<ContentfulRedirect>();
            collection.Items = new List<ContentfulRedirect>();

            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string>()), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void GetRedirect_StatusCodeSuccessful_WhenLegacyOrShortUrlAreAvailable()
        {
            var shortItems = new Dictionary<string, RedirectDictionary> { { "unittest", new RedirectDictionary { { "/short-test", "short-redirect-url" } } } };
            var legacyItems = new Dictionary<string, RedirectDictionary> { { "unittest", new RedirectDictionary { { "/legacy-test", "legacy-redirect-url" } } } };
            _shortUrlRedirects.Redirects = shortItems;
            _legacyUrlRedirects.Redirects = legacyItems;

            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GetRedirect_ShouldNotCallClient_WhenLegacyOrShortUrlAreAvailable()
        {
            var shortItems = new Dictionary<string, RedirectDictionary> { { "unittest", new RedirectDictionary { { "/short-test", "short-redirect-url" } } } };
            var legacyItems = new Dictionary<string, RedirectDictionary> { { "unittest", new RedirectDictionary { { "/legacy-test", "legacy-redirect-url" } } } };
            _shortUrlRedirects.Redirects = shortItems;
            _legacyUrlRedirects.Redirects = legacyItems;

            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

            AsyncTestHelper.Resolve(repository.GetRedirects());
            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

            _client.Verify(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void GetUpdatedRedirects_BusinessIdExist_ReturnSuccessful()
        {
            var ContentfulRedirects = new ContentfulRedirectBuilder().Build();
            var collection = new ContentfulCollection<ContentfulRedirect>();
            collection.Items = new List<ContentfulRedirect> { ContentfulRedirects };

            var redirectItem = new BusinessIdToRedirects(new Dictionary<string, string> { { "a-url", "another-url" } }, new Dictionary<string, string> { { "some-url", "another-url" } });

            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);
            _contenfulFactory.Setup(o => o.ToModel(ContentfulRedirects)).Returns(redirectItem);

            var response = AsyncTestHelper.Resolve(repository.GetUpdatedRedirects());

            var redirects = response.Get<Redirects>();

            var shortUrls = redirects.ShortUrlRedirects;
            shortUrls.Count.Should().Be(1);
            shortUrls.Keys.First().Should().Be("unittest");
            shortUrls["unittest"].ContainsKey("a-url").Should().BeTrue();
            var legacyUrls = redirects.LegacyUrlRedirects;
            legacyUrls.Count.Should().Be(1);
            legacyUrls.Keys.First().Should().Be("unittest");
            legacyUrls["unittest"].ContainsKey("some-url").Should().BeTrue();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GetUpdatedRedirects_NoBusinessId_ReturnNotFound()
        {
            var repository = new RedirectsRepository(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string>()), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

            var response = AsyncTestHelper.Resolve(repository.GetUpdatedRedirects());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}