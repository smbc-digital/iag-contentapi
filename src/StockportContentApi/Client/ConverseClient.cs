namespace StockportContentApi.Client;

public interface IConverseClient
{
    Task<string> GetLibraries();
}

public class ConverseClient : IConverseClient
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly ILogger<ConverseClient> _logger;

    public ConverseClient(
        System.Net.Http.HttpClient httpClient,
        ConverseConfig config,
        ILogger<ConverseClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.ApiKey);
        _logger = logger;
    }

    public async Task<string> GetLibraries()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(string.Empty);

        if (!response.IsSuccessStatusCode)
            _logger.LogError(
                "{Client}::{Method}: Converse returned {StatusCode} - {ReasonPhrase}",
                nameof(ConverseClient),
                nameof(GetLibraries),
                response.StatusCode,
                response.ReasonPhrase);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
