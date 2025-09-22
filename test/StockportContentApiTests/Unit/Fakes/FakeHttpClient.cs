namespace StockportContentApiTests.Unit.Fakes;

public class FakeHttpClient : IHttpClient
{
    private readonly Dictionary<string, object> _responses = new();
    private string _url;

    public FakeHttpClient For(string url)
    {
        _url = url;
        return this;
    }

    public void Return(HttpResponse response) =>
        _responses.Add(_url, response);

    public Task<HttpResponse> Get(string url)
    {
        if (!_responses.ContainsKey(url))
            throw new Exception($"The url requested was not stubbed in our fake http client: {url}");

        object response = _responses[url];
        Exception exception = response as Exception;

        if (exception is not null)
            throw exception;

        return Task.FromResult((HttpResponse)_responses[url]);
    }
}