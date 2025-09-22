namespace StockportContentApiTests.Unit.Helpers;

public class AuthenticationHelperTests
{
    private readonly AuthenticationHelper _helper;

    public AuthenticationHelperTests() =>
        _helper = new AuthenticationHelper();

    [Fact]
    public void ExtractAuthenticationDataFromContext_ShouldReturnAuthenticationDataWithCorrectValues()
    {
        // Arrange
        DefaultHttpContext context = new();
        context.Request.Path = "/v1/stockportgov/articles/test";
        context.Request.Headers.Append("Authorization", "test");
        context.Request.Method = "GET";

        // Act
        AuthenticationData data = _helper.ExtractAuthenticationDataFromContext(context);

        // Assert
        Assert.Equal(1, data.Version);
        Assert.Equal("test", data.AuthenticationKey);
        Assert.Equal("stockportgov", data.BusinessId);
        Assert.Equal("articles", data.Endpoint);
        Assert.Equal("GET", data.Verb);
        Assert.Equal("v1", data.VersionText);
    }

    [Theory]
    [InlineData("article", "articles")]
    [InlineData("payment", "payments")]
    [InlineData("event", "events")]
    [InlineData("topic", "topics")]
    [InlineData("profile", "profiles")]
    [InlineData("start-page", "start pages")]
    public void GetApiEndPoint_ShouldReturnCorrectEndpoint(string requestedEndpoint, string result)
    {
        // Act
        string returnedEndpoint = _helper.GetApiEndPoint(requestedEndpoint);

        // Assert
        Assert.Equal(result, returnedEndpoint);
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
}
