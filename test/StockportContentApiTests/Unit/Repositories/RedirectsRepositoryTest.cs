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
    public class RedirectsRepositoryTest
    {
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly ContentfulConfig _config;

        public RedirectsRepositoryTest()
        {
            _config = new ContentfulConfig("unittest")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("UNITTEST_SPACE", "SPACE")
                .Add("UNITTEST_ACCESS_KEY", "KEY")
                .Build();
        }

        [Fact]
        public void ItGetsListOfRedirectsBack()
        {
            var mockRedirectBuilder = new Mock<IFactory<RedirectDictionary>>();

            mockRedirectBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new RedirectDictionary());

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=redirect"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json")));

            var repository = new RedirectsRepository(httpClient.Object, mockRedirectBuilder.Object, o => _config, new RedirectBusinessIds(new List<string> { "unittest" }));

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            var redirects = response.Get<Dictionary<string, RedirectDictionary>>();
            redirects.Count.Should().Be(1);
            redirects.Keys.First().Should().Be("unittest");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ItGetsAnEmptyListForBusinessIdIfNoRedirectsFound()
        {
            var mockRedirectBuilder = new Mock<IFactory<RedirectDictionary>>();

            mockRedirectBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new RedirectDictionary());

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=redirect"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            var repository = new RedirectsRepository(httpClient.Object, mockRedirectBuilder.Object, o => _config, new RedirectBusinessIds(new List<string>() { "unittest" }));

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ItGets404BackForRedirects()
        {
            var mockRedirectBuilder = new Mock<IFactory<RedirectDictionary>>();

            mockRedirectBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new RedirectDictionary());

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=redirect"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            var repository = new RedirectsRepository(httpClient.Object, mockRedirectBuilder.Object, o => _config, new RedirectBusinessIds(new List<string>()));

            var response = AsyncTestHelper.Resolve(repository.GetRedirects());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
