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
    public class RedirectsRepositoryTest : TestingBaseClass

    {
    private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
    private readonly ContentfulConfig _config;

    public RedirectsRepositoryTest()
    {
        _config = new ContentfulConfig("unittest")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("UNITTEST_SPACE", "SPACE")
            .Add("UNITTEST_ACCESS_KEY", "KEY")
            .Add("UNITTEST_MANAGEMENT_KEY", "KEY")
            .Build();
    }

    [Fact]
    public void ItGetsListOfRedirectsBack()
    {
        var mockRedirectBuilder = new Mock<IFactory<BusinessIdToRedirects>>();

        mockRedirectBuilder.Setup(
                o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
            .Returns(new BusinessIdToRedirects(new Dictionary<string, string> {{"a-url", "another-url"}},
                new Dictionary<string, string>() {{"some-url", "some-other-url"}}));

        var httpClient = new Mock<IHttpClient>();
        httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=redirect"))
            .ReturnsAsync(HttpResponse.Successful(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Redirects.json")));

        var repository = new RedirectsRepository(httpClient.Object, mockRedirectBuilder.Object, o => _config,
            new RedirectBusinessIds(new List<string> {"unittest"}));

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
        var mockRedirectBuilder = new Mock<IFactory<BusinessIdToRedirects>>();

        mockRedirectBuilder.Setup(
                o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
            .Returns(new BusinessIdToRedirects(new Dictionary<string, string>(), new Dictionary<string, string>()));

        var httpClient = new Mock<IHttpClient>();
        httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=redirect"))
            .ReturnsAsync(
                HttpResponse.Successful(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.ContentNotFound.json")));

        var repository = new RedirectsRepository(httpClient.Object, mockRedirectBuilder.Object, o => _config,
            new RedirectBusinessIds(new List<string>() {"unittest"}));

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
        var mockRedirectBuilder = new Mock<IFactory<BusinessIdToRedirects>>();

        mockRedirectBuilder.Setup(
                o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
            .Returns(new BusinessIdToRedirects(new Dictionary<string, string>(), new Dictionary<string, string>()));

        var httpClient = new Mock<IHttpClient>();
        httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=redirect"))
            .ReturnsAsync(
                HttpResponse.Successful(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.ContentNotFound.json")));

        var repository = new RedirectsRepository(httpClient.Object, mockRedirectBuilder.Object, o => _config,
            new RedirectBusinessIds(new List<string>()));

        var response = AsyncTestHelper.Resolve(repository.GetRedirects());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    }
}
