namespace StockportContentApiTests.Unit.Http;

public class HttpClientTests
{
    private readonly Mock<IMsHttpClientWrapper> _msHttpClient = new();
    private readonly Mock<ILogger<HttpClient>> _logger = new();
    private readonly HttpClient _httpClient;

    public HttpClientTests() =>
        _httpClient = new HttpClient(_msHttpClient.Object, _logger.Object);

    [Fact]
    public async Task ShouldReturnFailureForHttpRequestException()
    {
        // Arrange
        _msHttpClient
            .Setup(msHttpClient => msHttpClient.GetAsync(It.IsAny<string>()))
            .Throws(new HttpRequestException());

        // Act
        HttpResponse response = await _httpClient.Get("http://www.nourl.com");

        // Assert
        LogTesting.Assert(_logger, LogLevel.Error, "An error occured while communicating with the remote service.");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForNon200StatusCode()
    {
        // Arrange
        HttpResponseMessage expectedMessage = new(HttpStatusCode.NotFound)
        {
            ReasonPhrase = "404"
        };

        _msHttpClient
            .Setup(msHttpClient => msHttpClient.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedMessage);

        // Act
        HttpResponse response = await _httpClient.Get("http://www.nourl.com");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("404", response.Error);
    }

    [Fact]
    public async Task ShouldReturnSuccessFor200StatusCode()
    {
        // Arrange
        HttpResponseMessage expectedMessage = new(HttpStatusCode.OK)
        {
            Content = new StringContent("200")
        };

        _msHttpClient
            .Setup(msHttpClient => msHttpClient.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedMessage);

        // Act
        HttpResponse response = await _httpClient.Get("http://www.nourl.com");
        string content = response.Get<string>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("200", content);
    }
}