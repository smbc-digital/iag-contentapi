namespace StockportContentApiTests.Unit.Client;

public class ContentfulClientManagerTests
{
    [Fact]
    public void ShouldReturnClient()
    {
        Mock<IConfiguration> _configuration = new();
        System.Net.Http.HttpClient httpClient = new();
        ContentfulClientManager manager = new(httpClient, _configuration.Object);
        ContentfulConfig config = new ContentfulConfig("test")
           .Add("DELIVERY_URL", "https://test.url")
           .Add("TEST_SPACE", "SPACE")
           .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
           .Build();

        IContentfulClient contenfulClient = manager.GetClient(config);
        
        contenfulClient.Should().BeEquivalentTo(new Contentful.Core.ContentfulClient(httpClient, "", config.AccessKey, config.SpaceKey));
    }
}
