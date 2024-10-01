namespace StockportContentApiTests.Unit.Helpers;

public class AuthenticationHelperTests
{
    private readonly AuthenticationHelper _helper;

    public AuthenticationHelperTests()
    {
        _helper = new AuthenticationHelper();
    }

    [Fact]
    public void ExtractAuthenticationDataFromContext_ShouldReturnAuthenticationDataWithCorrectValues()
    {
        // Arrange
        DefaultHttpContext context = new();
        context.Request.Path = "/v1/stockportgov/articles/test";
        context.Request.Headers.Add("Authorization", "test");
        context.Request.Method = "GET";

        // Act
        AuthenticationData data = _helper.ExtractAuthenticationDataFromContext(context);

        // Assert
        data.Version.Should().Be(1);
        data.AuthenticationKey.Should().Be("test");
        data.BusinessId.Should().Be("stockportgov");
        data.Endpoint.Should().Be("articles");
        data.Verb.Should().Be("GET");
        data.VersionText.Should().Be("v1");
    }

    [Theory]
    [InlineData("article", "articles")]
    [InlineData("group", "groups")]
    [InlineData("payment", "payments")]
    [InlineData("event", "events")]
    [InlineData("topic", "topics")]
    [InlineData("profile", "profiles")]
    [InlineData("start-page", "start pages")]
    [InlineData("organisation", "organisations")]
    public void GetApiEndPoint_ShouldReturnCorrectEndpoint(string requestedEndpoint, string result)
    {
        // Act
        string returnedEndpoint = _helper.GetApiEndPoint(requestedEndpoint);

        // Assert
        returnedEndpoint.Should().Be(result);
    }

    [Fact]
    public void CheckVersionIsProvided_ShouldThrowExceptionIfVersionIsNotProvided()
    {
        // Arrange
        AuthenticationData authData = new()
        {
            AuthenticationKey = "key",
            BusinessId = "businessid",
            Version = 1,
            Endpoint = "endpoint",
            Verb = "GET",
            VersionText = "incorrectText"
        };

        // Act Assert
        Assert.Throws<AuthorizationException>(() => _helper.CheckVersionIsProvided(authData));
    }

    [Fact]
    public void Invoke_ShouldReturnIfNoApiKeyIsInTheConfig()
    {
        // Arrange
        DefaultHttpContext context = new();
        context.Request.Path = "/v1/stockportgov/articles/test";
        context.Request.Headers.Add("Authorization", "test");
        context.Request.Method = "GET";
    }
}
