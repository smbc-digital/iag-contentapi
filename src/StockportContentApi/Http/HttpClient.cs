﻿namespace StockportContentApi.Http;

public interface IHttpClient
{
    Task<HttpResponse> Get(string url);
}

public class HttpClient : IHttpClient
{
    private readonly IMsHttpClientWrapper _client;
    private readonly ILogger<HttpClient> _logger;

    public HttpClient(IMsHttpClientWrapper client, ILogger<HttpClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<HttpResponse> Get(string url)
    {
        try
        {
            var responseMessage = await _client.GetAsync(url);

            if (!responseMessage.IsSuccessStatusCode) return HttpResponse.Failure(responseMessage.StatusCode, responseMessage.ReasonPhrase);

            var content = await responseMessage.Content.ReadAsStringAsync();

            return HttpResponse.Successful(content);
        }
        catch (HttpRequestException ex)
        {
            // GetAsync above throws an exception, catch it here and return fail
            _logger.LogError(new EventId(0), ex, "An error occured while communicating with the remote service.");
            return HttpResponse.Failure(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}