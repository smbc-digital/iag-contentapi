namespace StockportContentApiTests.Unit.Client;

public class ContentfulClientManagerTest
{
    [Fact]
    public void ShouldReturnClient()
    {
        var _configuration = new Mock<IConfiguration>();
        var httpClient = new System.Net.Http.HttpClient();
        var manager = new ContentfulClientManager(httpClient, _configuration.Object);
        var config = new ContentfulConfig("test")
           .Add("DELIVERY_URL", "https://test.url")
           .Add("TEST_SPACE", "SPACE")
           .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
           .Build();

        var contenfulClient = manager.GetClient(config);

        contenfulClient.Should().BeEquivalentTo(new Contentful.Core.ContentfulClient(httpClient, "", config.AccessKey, config.SpaceKey));
    }
}
