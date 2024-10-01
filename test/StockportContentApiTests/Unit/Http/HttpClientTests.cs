namespace StockportContentApiTests.Unit.Http;

public class HttpClientTests
{
    private readonly Mock<IMsHttpClientWrapper> _msHttpClient;
    private readonly Mock<ILogger<HttpClient>> _logger;
    private readonly HttpClient _httpClient;

    public HttpClientTests()
    {
        _msHttpClient = new Mock<IMsHttpClientWrapper>();
        _logger = new Mock<ILogger<HttpClient>>();
        _httpClient = new HttpClient(_msHttpClient.Object, _logger.Object);
    }

    [Fact]
    public void ShouldReturnFailureForHttpRequestException()
    {
        _msHttpClient.Setup(o => o.GetAsync(It.IsAny<string>())).Throws(new HttpRequestException());

        Task<HttpResponse> response = _httpClient.Get("http://www.nourl.com");

        LogTesting.Assert(_logger, LogLevel.Error, "An error occured while communicating with the remote service.");
        response.Result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public void ShouldReturnNotFoundForNon200StatusCode()
    {
        HttpResponseMessage expectedMessage = new(HttpStatusCode.NotFound)
        {
            ReasonPhrase = "404"
        };

        _msHttpClient.Setup(o => o.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedMessage);

        HttpResponse response = AsyncTestHelper.Resolve(_httpClient.Get("http://www.nourl.com"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be("404");
    }

    [Fact]
    public void ShouldReturnSuccessFor200StatusCode()
    {
        HttpResponseMessage expectedMessage = new(HttpStatusCode.OK)
        {
            Content = new StringContent("200")
        };

        _msHttpClient.Setup(o => o.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedMessage);

        HttpResponse response = AsyncTestHelper.Resolve(_httpClient.Get("http://www.nourl.com"));
        string content = response.Get<string>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("200");
    }
}
