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
    public class RedirectBuilderTest
    {
        private readonly IFactory<RedirectDictionary> _redirectFactory;

        public RedirectBuilderTest()
        {
            _redirectFactory = new RedirectsFactory();
        }

        [Fact]
        public void BuildAllRedirects()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            RedirectDictionary redirectDictionary = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            redirectDictionary.Count.Should().Be(5);
        }

        [Fact]
        public void TestRedirectGoesToBinsPage()
        {
            dynamic mockContentfulData =
               JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            RedirectDictionary redirectDictionary = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            string value;
            redirectDictionary.TryGetValue("starturl.fake/bins", out value);
            value.Should().Be("redirecturl.fake/bins");
        }

        [Fact]
        public void TestHasNoRedirect()
        {
            dynamic mockContentfulData =
               JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Redirects.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            RedirectDictionary redirectDictionary = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            string value;
            redirectDictionary.TryGetValue("starturl.fake/doesnotexist", out value);
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void TestNoContent()
        {
            dynamic mockContentfulData =
               JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            RedirectDictionary redirectDictionary = _redirectFactory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            string value;
            redirectDictionary.TryGetValue("starturl.fake/doesnotexist", out value);
            value.Should().BeNullOrEmpty();
            redirectDictionary.Should().BeOfType<NullRedirectDictionary>();
        }

    }
}
