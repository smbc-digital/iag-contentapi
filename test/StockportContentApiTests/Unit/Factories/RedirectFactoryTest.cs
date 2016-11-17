using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class RedirectFactoryTest
    {
        private readonly IFactory<BusinessIdToRedirects> _redirectFactory;

        public RedirectFactoryTest()
        {
            _redirectFactory = new RedirectsFactory();
        }

        [Fact]
        public void BuildAllRedirects()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            BusinessIdToRedirects redirects = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            redirects.ShortUrlRedirects.Count.Should().Be(5);
            redirects.ShortUrlRedirects.Should().BeOfType<RedirectDictionary>();
            redirects.LegacyUrlRedirects.Count.Should().Be(2);
            redirects.LegacyUrlRedirects.Should().BeOfType<RedirectDictionary>();
        }

        [Fact]
        public void TestRedirectHasCorrectKeysAndValues()
        {
            dynamic mockContentfulData =
               JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            BusinessIdToRedirects redirects = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            string shortUrlRedirectsValue;
            redirects.ShortUrlRedirects.TryGetValue("starturl.fake/bins", out shortUrlRedirectsValue);
            shortUrlRedirectsValue.Should().Be("redirecturl.fake/bins");

            string legacyUrlRedirectsValue;
            redirects.LegacyUrlRedirects.TryGetValue("a-url", out legacyUrlRedirectsValue);
            legacyUrlRedirectsValue.Should().Be("another-url");
        }

        [Fact]
        public void TestHasNoRedirect()
        {
            dynamic mockContentfulData =
               JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            BusinessIdToRedirects redirects = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            string shortUrlRedirectsValue;
            redirects.ShortUrlRedirects.TryGetValue("starturl.fake/doesnotexist", out shortUrlRedirectsValue);
            shortUrlRedirectsValue.Should().BeNullOrEmpty();

            string legacyUrlRedirectsValue;
            redirects.LegacyUrlRedirects.TryGetValue("starturl.fake/doesnotexist", out legacyUrlRedirectsValue);
            legacyUrlRedirectsValue.Should().BeNullOrEmpty();
        }

        [Fact]
        public void TestNoContent()
        {
            dynamic mockContentfulData =
               JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            BusinessIdToRedirects redirects = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            string shortUrlRedirectsValue;
            redirects.ShortUrlRedirects.TryGetValue("starturl.fake/doesnotexist", out shortUrlRedirectsValue);
            shortUrlRedirectsValue.Should().BeNullOrEmpty();

            string legacyUrlRedirectsValue;
            redirects.LegacyUrlRedirects.TryGetValue("starturl.fake/doesnotexist", out legacyUrlRedirectsValue);
            legacyUrlRedirectsValue.Should().BeNullOrEmpty();
        }
    }
}
