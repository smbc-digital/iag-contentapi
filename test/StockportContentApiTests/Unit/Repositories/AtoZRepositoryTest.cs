using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using IContentfulClient = Contentful.Core.IContentfulClient;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Search;
using File = System.IO.File;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.Repositories
{
    public class AtoZRepositoryTest
    {
        private readonly Mock<IContentfulClient> _client;
        private readonly ContentfulConfig _config;
        private readonly Mock<IContentfulClientManager> _contentfulClientManager;
        private readonly Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>> _aToZFactory;
        //private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";

        public AtoZRepositoryTest()
        {
            _config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
            _aToZFactory = new Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>>();          
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterV()
        {
            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
                {
                    new ContentfulAToZBuilder().Title("Vintage Village 1").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 2").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 3").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 4").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 5").Build(),
                };

            _client.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<ContentfulAtoZ>>(q => q.Build() == new QueryBuilder<ContentfulAtoZ>().ContentTypeIs("article").Include(2).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);
            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
                .Returns(new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old",
                    "The vintage village turned 6 with a great reception", "article", new List<string>()));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("v"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var aToZListing = response.Get<List<AtoZ>>();
            aToZListing.Count.Should().Be(5);
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterB()
        {
            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
                {
                    new ContentfulAToZBuilder().Title("Bintage Village 1").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 2").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 3").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 4").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 5").Build(),
                };
            _client.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<ContentfulAtoZ>>(q => q.Build() == new QueryBuilder<ContentfulAtoZ>().ContentTypeIs("article").Include(2).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
               .Returns(new AtoZ("Bintage Village turns 6 years old", "bintage-village-turns-6-years-old",
                   "The vintage village turned 6 with a great reception", "article", new List<string>()));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null);
            var response = AsyncTestHelper.Resolve(repository.Get("b"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var aToZListing = response.Get<List<AtoZ>>();

            aToZListing.Count.Should().Be(5);
        }

        [Fact]
        public void ItReturnsANotFoundIfNoItemsMatch()
        {
            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
                {
                    new ContentfulAToZBuilder().Title("Because a Vintage Village turns 6 years old").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 2").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 3").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 4").Build(),
                    new ContentfulAToZBuilder().Title("Bintage Village 5").Build(),
                };
            _client.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<ContentfulAtoZ>>(q => q.Build() == new QueryBuilder<ContentfulAtoZ>().ContentTypeIs("article").Include(2).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
               .Returns(new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old",
                   "The vintage village turned 6 with a great reception", "article", new List<string>()));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("d"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No results found");
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterBWhereTheLetterMatchesWithAnAlterniveTitleAndSetsTheTitleAsTheAlternativeTitle()
        {
            var alternativeTitle = "Do you know this started!";

            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
                {
                    new ContentfulAToZBuilder().Title("Vintage Village 1").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 2").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 3").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 4").Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 5").Build(),
                };

            _client.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<ContentfulAtoZ>>(q => q.Build() == new QueryBuilder<ContentfulAtoZ>().ContentTypeIs("article").Include(2).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);
            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
                .Returns(new AtoZ("Because a Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string>() { alternativeTitle }));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("d"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var aToZListing = response.Get<List<AtoZ>>();
            aToZListing.Count.Should().Be(5);
            aToZListing.FirstOrDefault().Title.Should().Be(alternativeTitle);
        }

        [Fact]
        public void ItGetsAnAtoZListingItemWithMultipleAlternateTitles()
        {
            var alternateTitles = new List<string> { "This is alternate title", "this is also another alternate title" };

            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
                {
                    new ContentfulAToZBuilder().Title("Vintage Village").Build(),
                };

            _client.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<ContentfulAtoZ>>(q => q.Build() == new QueryBuilder<ContentfulAtoZ>().ContentTypeIs("article").Include(2).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);
            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
                .Returns(new AtoZ("title", "slug", "teaser", "article", alternateTitles));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("t"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = response.Get<List<AtoZ>>();
            result.Count.Should().Be(3);
            result[0].Title.Should().Be("this is also another alternate title");
            result[1].Title.Should().Be("This is alternate title");
            result[2].Title.Should().Be("title");
        }
    }
}
