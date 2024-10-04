namespace StockportContentApi.Http;

public class LoggingHttpClient : IHttpClient
{
    private readonly IHttpClient _inner;
    private readonly ILogger<LoggingHttpClient> _logger;

    public LoggingHttpClient(IHttpClient inner,
        ILogger<LoggingHttpClient> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<HttpResponse> Get(string url)
    {
        _logger.LogInformation($"Querying: {RemoveAccessKeyFromUrl(url)}");
        HttpResponse response = await _inner.Get(url);
        _logger.LogDebug($"Response: {response}");

        return response;
    }

    private static string RemoveAccessKeyFromUrl(string url)
    {
        const string accessToken = "access_token";
        if (string.IsNullOrWhiteSpace(url) || !url.Contains(accessToken))
            return url;

        string token = url.Split('&')
            .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s) && s.Contains(accessToken))?
            .Split('=')[1];

        if (!string.IsNullOrEmpty(token))
            url = url.Replace(token, "*****");

        return url;
    }
}