namespace StockportContentApi.Http;

public interface IHttpClient
{
    Task<HttpResponse> Get(string url);
}

[ExcludeFromCodeCoverage]
public class HttpClient(IMsHttpClientWrapper client, ILogger<HttpClient> logger) : IHttpClient
{
    private readonly IMsHttpClientWrapper _client = client;
    private readonly ILogger<HttpClient> _logger = logger;

    public async Task<HttpResponse> Get(string url)
    {
        try
        {
            HttpResponseMessage responseMessage = await _client.GetAsync(url);

            if (!responseMessage.IsSuccessStatusCode)
                return HttpResponse.Failure(responseMessage.StatusCode, responseMessage.ReasonPhrase);

            string content = await responseMessage.Content.ReadAsStringAsync();

            return HttpResponse.Successful(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(new EventId(0), ex, "An error occured while communicating with the remote service.");
            return HttpResponse.Failure(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}