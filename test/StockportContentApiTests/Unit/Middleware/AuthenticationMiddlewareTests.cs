namespace StockportContentApiTests.Unit.Middleware;

public class AuthenticationMiddlewareTests
{
    private readonly AuthenticationMiddleware _middleware;
    private readonly Mock<RequestDelegate> _requestDelegate;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<ILogger<AuthenticationMiddleware>> _logger;
    private readonly Mock<IAuthenticationHelper> _authHelper;
    private readonly Mock<Func<string, ContentfulConfig>> _createConfig;

    public AuthenticationMiddlewareTests()
    {
        _createConfig = new Mock<Func<string, ContentfulConfig>>();
        _configuration = new Mock<IConfiguration>();
        _requestDelegate = new Mock<RequestDelegate>();
        _logger = new Mock<ILogger<AuthenticationMiddleware>>();
        _authHelper = new Mock<IAuthenticationHelper>();
        _middleware = new AuthenticationMiddleware(_requestDelegate.Object, _configuration.Object, _logger.Object, _authHelper.Object, _createConfig.Object);
    }

    [Fact]
    public async Task Invoke_ShouldReturnIfNoApiKeyIsInTheConfig()
    {
        // Arrange
        DefaultHttpContext context = new();
        context.Request.Path = "/v1/stockportgov/articles/test";
        context.Request.Headers.Append("Authorization", "test");
        context.Request.Method = "GET";

        // Act
        await _middleware.Invoke(context);

        // Assert
        _authHelper.Verify(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_ShouldReturnIfNoApiKeyIsInTheRequest()
    {
        // Arrange
        DefaultHttpContext context = new();
        AuthenticationData authData = new()
        {
            AuthenticationKey = "key",
            BusinessId = "businessid",
            Version = 1,
            Endpoint = "endpoint",
            Verb = "GET",
            VersionText = "incorrectText"
        };
        _authHelper.Setup(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>())).Returns(authData);
        _configuration.Setup(_ => _["Authorization"]).Returns("key");

        // Act
        await _middleware.Invoke(context);

        // Assert
        _authHelper.Verify(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>()), Times.Once);
    }

    [Fact]
    public async Task Invoke_ShouldInvokeNextIfKeysMatch()
    {
        // Arrange
        DefaultHttpContext context = new();
        AuthenticationData authData = new()
        {
            AuthenticationKey = "key",
            BusinessId = "businessid",
            Version = 1,
            Endpoint = "endpoint",
            Verb = "GET",
            VersionText = "incorrectText"
        };
        _authHelper.Setup(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>())).Returns(authData);
        _configuration.Setup(_ => _["Authorization"]).Returns("key");

        // Act
        await _middleware.Invoke(context);

        // Assert
        _requestDelegate.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
    }
}
