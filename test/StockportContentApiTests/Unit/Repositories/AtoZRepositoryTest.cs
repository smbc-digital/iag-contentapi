using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class AtoZRepositoryTest
    {
        private readonly Mock<IContentfulClient> _client;
        private readonly ContentfulConfig _config;
        private readonly Mock<IContentfulClientManager> _contentfulClientManager;
        private readonly Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>> _aToZFactory;
        private readonly Mock<ICache> _cache;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger> _logger;

        //private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";

        public AtoZRepositoryTest()
        {
            _config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:AtoZ"]).Returns("60");
            _cache = new Mock<ICache>();

            _contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
            _aToZFactory = new Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>>();
            _logger = new Mock<ILogger>();
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterV()
        {
            var letter = "v";
            var aToZArticle = new List<AtoZ>
            {
                new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> {"V atoztitle"}),
                new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> {"V atoztitle"}),
                new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> {"V atoztitle"})
            };

            var aToZShowcase = new List<AtoZ>
            {
                new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "showcase", new List<string> {"V atoztitle"}),
                new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "showcase", new List<string> {"V atoztitle"}),
                new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "showcase", new List<string> {"V atoztitle"})
            };

            var aToZTopic = new List<AtoZ>
            {
                new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> {"V atoztitle"}),
                new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> {"V atoztitle"}),
                new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> {"V atoztitle"})
            };

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
                .Returns(new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old",
                    "The vintage village turned 6 with a great reception", "article", new List<string>()));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-article-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(aToZArticle);
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-topic-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(aToZTopic);
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-showcase-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(aToZShowcase);

            var response = AsyncTestHelper.Resolve(repository.Get(letter));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var aToZListing = response.Get<List<AtoZ>>();
            aToZListing.Count.Should().Be(9);
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterB()
        {
            var letter = "b";
            var aToZArticle = new List<AtoZ>
            {
                new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> {"V atoztitle"}),
                new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> {"V atoztitle"}),
                new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> {"V atoztitle"})
            };

            var aToZShowcase = new List<AtoZ>
            {
                new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "showcase", new List<string> {"V atoztitle"}),
                new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "showcase", new List<string> {"V atoztitle"}),
                new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "showcase", new List<string> {"V atoztitle"})
            };

            var aToZTopic = new List<AtoZ>
            {
                new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> {"V atoztitle"}),
                new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> {"V atoztitle"}),
                new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> {"V atoztitle"})
            };

            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-article-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(aToZArticle);
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-topic-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(aToZTopic);
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-showcase-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(aToZShowcase);

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
               .Returns(new AtoZ("Bintage Village turns 6 years old", "bintage-village-turns-6-years-old",
                   "The vintage village turned 6 with a great reception", "article", new List<string>()));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);
            var response = AsyncTestHelper.Resolve(repository.Get("b"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var aToZListing = response.Get<List<AtoZ>>();

            aToZListing.Count.Should().Be(9);
        }

        [Fact]
        public void ItReturnsANotFoundIfNoItemsMatch()
        {
            var letter = "b";           
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-article-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<AtoZ>());
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-topic-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<AtoZ>());
            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"atoz-showcase-{letter}"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<AtoZ>());

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
               .Returns(new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old",
                   "The vintage village turned 6 with a great reception", "article", new List<string>()));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

            var response = AsyncTestHelper.Resolve(repository.Get("b"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No results found");
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterBWhereTheLetterMatchesWithAnAlterniveTitleAndSetsTheTitleAsTheAlternativeTitle()
        {
            var alternativeTitle = "Do you know this started!";

            var nullAToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            nullAToZcollection.Items = new List<ContentfulAtoZ>();

            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
                {
                    new ContentfulAToZBuilder().Title("Vintage Village 1").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 2").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 3").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 4").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
                    new ContentfulAToZBuilder().Title("Vintage Village 5").AlternativeTitles(new List<string> { alternativeTitle }).Build()
                };

            _client.Setup(o => o.GetEntries<ContentfulAtoZ>("?content_type=article&include=0&limit=1000&skip=0",
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);

            _client.Setup(o => o.GetEntries<ContentfulAtoZ>("?content_type=topic&include=0&limit=1000&skip=0",
                              It.IsAny<CancellationToken>())).ReturnsAsync(nullAToZcollection);

            _client.Setup(o => o.GetEntries<ContentfulAtoZ>("?content_type=showcase&include=0&limit=1000&skip=0",
                             It.IsAny<CancellationToken>())).ReturnsAsync(nullAToZcollection);

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
                .Returns(new AtoZ("Because a Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string> { alternativeTitle }));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

            var aToZListing = AsyncTestHelper.Resolve(repository.GetAtoZItemFromContentType("article","d"));
           
            aToZListing.Count.Should().Be(5);
            aToZListing.FirstOrDefault().Title.Should().Be(alternativeTitle);
        }

        [Fact]
        public void ItGetsAnAtoZListingItemWithMultipleAlternateTitles()
        {
            var alternateTitles = new List<string> { "This is alternate title", "this is also another alternate title" };

            var nullAToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            nullAToZcollection.Items = new List<ContentfulAtoZ>();

            var aToZcollection = new ContentfulCollection<ContentfulAtoZ>();
            aToZcollection.Items = new List<ContentfulAtoZ>
            {
                new ContentfulAToZBuilder().Title("Vintage Village").AlternativeTitles(alternateTitles).Build()
            };

            _client.Setup(o=>o.GetEntries<ContentfulAtoZ>("?content_type=article&include=0&limit=1000&skip=0",
                               It.IsAny<CancellationToken>())).ReturnsAsync(aToZcollection);

            _client.Setup(o => o.GetEntries<ContentfulAtoZ>("?content_type=topic&include=0&limit=1000&skip=0",
                            It.IsAny<CancellationToken>())).ReturnsAsync(nullAToZcollection);

            _client.Setup(o => o.GetEntries<ContentfulAtoZ>("?content_type=showcase&include=0&limit=1000&skip=0",
                             It.IsAny<CancellationToken>())).ReturnsAsync(nullAToZcollection);

            _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
                .Returns(new AtoZ("title", "slug", "teaser", "article", alternateTitles));
            var repository = new AtoZRepository(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

            var result = AsyncTestHelper.Resolve(repository.GetAtoZItemFromContentType("article", "t"));
          
            result.Count.Should().Be(3);           
            result[0].Title.Should().Be("title");
            result[1].Title.Should().Be("This is alternate title");
            result[2].Title.Should().Be("this is also another alternate title");
        }
    }
}
