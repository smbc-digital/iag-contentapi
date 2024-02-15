namespace StockportContentApi.Http;

public interface IMsHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string url);
}

public class MsHttpClientWrapper : IMsHttpClientWrapper
{
    private readonly System.Net.Http.HttpClient _client;
    private readonly bool _requiresProxy;

    public MsHttpClientWrapper(bool requiresProxy=false)
    {
        _requiresProxy = requiresProxy;

        if(requiresProxy)
        {
            _client = new System.Net.Http.HttpClient(new HttpClientHandler
                                                        { 
                                                            Proxy = new WebProxy("172.16.0.166", 8080)
                                                        });
        }
        else
        {
            _client = new System.Net.Http.HttpClient();
        }
    }

    public Task<HttpResponseMessage> GetAsync(string url)
    {
        return _client.GetAsync(url);
    }
}
