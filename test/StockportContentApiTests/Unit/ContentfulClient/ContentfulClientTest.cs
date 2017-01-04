using System.IO;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Http;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulClient
{
    public class ContentfulClientTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly StockportContentApi.ContentfulClient _contentfulClient;

        public ContentfulClientTest()
        {
            _httpClient = new Mock<IHttpClient>();
            _contentfulClient = new StockportContentApi.ContentfulClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldReturnNullContenfulResponseForNonOkHttpResponsse()
        {
            _httpClient.Setup(o => o.Get("not-found")).ReturnsAsync(HttpResponse.Failure(HttpStatusCode.NotFound, "not found"));

            var response = AsyncTestHelper.Resolve(_contentfulClient.Get("not-found"));

            response.Should().BeOfType<NullContentfulResponse>();
            response.HasItems().Should().Be(false);
        }

        [Fact]
        public void ShouldReturnContentfulResponseWithItemsForOkHttpResponse()
        {
            _httpClient.Setup(o => o.Get("good-url")).ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));

            var response = AsyncTestHelper.Resolve(_contentfulClient.Get("good-url"));

            response.Should().BeOfType<ContentfulResponse>();

            response.HasItems().Should().Be(true);
        }
    }
}
