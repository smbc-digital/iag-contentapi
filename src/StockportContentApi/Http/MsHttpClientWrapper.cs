namespace StockportContentApi.Http;

public interface IMsHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string url);
}

[ExcludeFromCodeCoverage]
public class MsHttpClientWrapper : IMsHttpClientWrapper
{
    private readonly System.Net.Http.HttpClient _client = new();

    public Task<HttpResponseMessage> GetAsync(string url)
        => _client.GetAsync(url);
}
