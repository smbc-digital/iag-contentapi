using System.Net.Http;
using FluentAssertions;
using StockportContentApi.Client;
using StockportContentApi.Config;
using Xunit;

namespace StockportContentApiTests.Unit.Client
{
    public class ContentfulClientManagerTest
    {
        [Fact]
        public void ShouldReturnClient()
        {
            var httpClient = new HttpClient();
            var manager = new ContentfulClientManager(httpClient);
            var config = new ContentfulConfig("test")
               .Add("DELIVERY_URL", "https://test.url")
               .Add("TEST_SPACE", "SPACE")
               .Add("TEST_ACCESS_KEY", "KEY")
               .Build();

            var contenfulClient = manager.GetClient(config);

            contenfulClient.ShouldBeEquivalentTo(new Contentful.Core.ContentfulClient(httpClient, config.AccessKey, config.SpaceKey) { ResolveEntriesSelectively = true });
        }
    }
}
