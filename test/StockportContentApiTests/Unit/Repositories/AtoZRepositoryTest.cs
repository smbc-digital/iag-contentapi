using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class AtoZRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly ContentfulConfig _config;

        public AtoZRepositoryTest()
        {
            _config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&fields.displayOnAZ=true"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/AtoZ.json")));
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=topic&fields.displayOnAZ=true"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/AtoZTopic.json")));
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterV()
        {
            var mockAtoZBuilder = new Mock<IFactory<AtoZ>>();

            mockAtoZBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new AtoZ ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string>()));

            var repository = new AtoZRepository(_config, _httpClient.Object, mockAtoZBuilder.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("v"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var aToZListing = response.Get<List<AtoZ>>();

            aToZListing.Count.Should().Be(5);
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterB()
        {
            var mockAtoZBuilder = new Mock<IFactory<AtoZ>>();

            mockAtoZBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new AtoZ("B Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string>()));

            var repository = new AtoZRepository(_config, _httpClient.Object, mockAtoZBuilder.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("b"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var aToZListing = response.Get<List<AtoZ>>();

            aToZListing.Count.Should().Be(5);
        }

        [Fact]
        public void ItReturnsANotFoundIfNoItemsMatch()
        {
            var mockAtoZBuilder = new Mock<IFactory<AtoZ>>();

            mockAtoZBuilder
                .Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new AtoZ("Because a Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string>() { "Shall you know this started!" }));

            var repository = new AtoZRepository(_config, _httpClient.Object, mockAtoZBuilder.Object, null);

            var response = AsyncTestHelper.Resolve(repository.Get("d"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No results found");
        }

        [Fact]
        public void ItGetsAnAtoZListingForTheLetterBWhereTheLetterMatchesWithAnAlterniveTitleAndSetsTheTitleAsTheAlternativeTitle()
        {
            var alternativeTitle = "Do you know this started!";

            var mockAtoZBuilder = new Mock<IFactory<AtoZ>>();
            mockAtoZBuilder
                .Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new AtoZ("Because a Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string>() { alternativeTitle }));

            var repository = new AtoZRepository(_config, _httpClient.Object, mockAtoZBuilder.Object, null);

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

            var mockAtoZBuilder = new Mock<IFactory<AtoZ>>();
            mockAtoZBuilder
                .Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new AtoZ("title", "slug", "teaser", "article", alternateTitles));

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&fields.displayOnAZ=true"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/AtoZSingleItem.json")));

            var repository = new AtoZRepository(_config, httpClient.Object, mockAtoZBuilder.Object, null);

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
