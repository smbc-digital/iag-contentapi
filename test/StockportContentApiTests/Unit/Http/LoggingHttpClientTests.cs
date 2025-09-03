namespace StockportContentApiTests.Unit.Http;

public class LoggingHttpClientTests
{
    private readonly FakeHttpClient _fakeHttpClient = new();
    private readonly FakeLogger<LoggingHttpClient> _fakeLogger = new();

    [Fact]
    public async Task HandlesSuccessFromRemote()
    {
        // Arrange
        _fakeHttpClient
            .For("a url")
            .Return(HttpResponse.Successful("some data"));

        // Act
        LoggingHttpClient httpClient = new(_fakeHttpClient, _fakeLogger);
        HttpResponse response = await httpClient.Get("a url");

        // Assert
        Assert.Equal("Querying: a url", _fakeLogger.InfoMessage);
        Assert.Equal($"Response: {response}", _fakeLogger.DebugMessage);
        Assert.Null(_fakeLogger.ErrorMessage);
    }

    [Fact]
    public async Task DoesNotLogAccessKey()
    {
        // Arrange
        string urlWithKey = "https://fake.url/spaces/SPACE/entries?access_token=KEY&content_type=topic";

        _fakeHttpClient
            .For(urlWithKey)
            .Return(HttpResponse.Successful("A response"));

        // Act
        LoggingHttpClient httpClient = new(_fakeHttpClient, _fakeLogger);
        await httpClient.Get(urlWithKey);

        // Assert
        Assert.DoesNotContain("KEY", _fakeLogger.InfoMessage);
        Assert.Contains("access_token=*****", _fakeLogger.InfoMessage);
        Assert.Equal("Querying: https://fake.url/spaces/SPACE/entries?access_token=*****&content_type=topic", _fakeLogger.InfoMessage);
    }
}