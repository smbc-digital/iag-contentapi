namespace StockportContentApiTests.Unit.Http;

public class LoggingHttpClientTests
{
    private readonly FakeHttpClient _fakeHttpClient = new();
    private readonly FakeLogger<LoggingHttpClient> _fakeLogger = new();

    [Fact]
    public void HandlesSuccessFromRemote()
    {
        _fakeHttpClient.For("a url")
            .Return(HttpResponse.Successful("some data"));

        LoggingHttpClient httpClient = new(_fakeHttpClient, _fakeLogger);
        HttpResponse response = AsyncTestHelper.Resolve(httpClient.Get("a url"));

        Assert.Equal("Querying: a url", _fakeLogger.InfoMessage);
        Assert.Equal($"Response: {response}", _fakeLogger.DebugMessage);
        Assert.Null(_fakeLogger.ErrorMessage);
    }

    [Fact]
    public async void DoesNotLogAccessKey()
    {
        string urlWithKey = "https://fake.url/spaces/SPACE/entries?access_token=KEY&content_type=topic";

        _fakeHttpClient.For(urlWithKey).Return(HttpResponse.Successful("A response"));

        LoggingHttpClient httpClient = new(_fakeHttpClient, _fakeLogger);
        await httpClient.Get(urlWithKey);

        Assert.DoesNotContain("KEY", _fakeLogger.InfoMessage);
        Assert.Contains("access_token=*****", _fakeLogger.InfoMessage);
        Assert.Equal("Querying: https://fake.url/spaces/SPACE/entries?access_token=*****&content_type=topic",
            _fakeLogger.InfoMessage);
    }
}