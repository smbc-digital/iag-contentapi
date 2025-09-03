using Contentful.Core;

namespace StockportContentApiTests.Unit.Client;

public class ContentfulClientManagerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration = new();
    private readonly System.Net.Http.HttpClient _httpClient = new();
    private readonly ContentfulClientManager _clientManager;

    public ContentfulClientManagerTests() =>
        _clientManager = new ContentfulClientManager(_httpClient, _mockConfiguration.Object);

    [Fact]
    public void GetClient_ShouldReturnContentfulClient_WhenUsePreviewApiIsFalse()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Contentful:UsePreviewAPI"]).Returns("false");
        ContentfulConfig config = new("stockportgov")
        {
            SpaceKey = "space-id",
            Environment = "environment-id",
            AccessKey = "access-key"
        };

        // Act
        IContentfulClient client = _clientManager.GetClient(config);

        // Assert
        Assert.NotNull(client);
        Assert.IsAssignableFrom<IContentfulClient>(client);
    }

    [Fact]
    public void GetClient_ShouldReturnContentfulClient_WithPreviewKey_WhenUsePreviewApiIsTrue()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Contentful:UsePreviewAPI"]).Returns("true");
        ContentfulConfig config = new("stockportgov")
        {
            SpaceKey = "space-id",
            Environment = "environment-id",
            AccessKey = "preview-api-key"
        };

        // Act
        IContentfulClient client = _clientManager.GetClient(config);

        // Assert
        Assert.NotNull(client);
        Assert.IsAssignableFrom<IContentfulClient>(client);
    }

    [Fact]
    public void GetManagementClient_ShouldReturnContentfulManagementClient()
    {
        // Arrange
        ContentfulConfig config = new("stockportgov")
        {
            SpaceKey = "space-id",
            ManagementKey = "management-api-key"
        };

        // Act
        IContentfulManagementClient managementClient = _clientManager.GetManagementClient(config);

        // Assert
        Assert.NotNull(managementClient);
        Assert.IsAssignableFrom<IContentfulManagementClient>(managementClient);
    }
}