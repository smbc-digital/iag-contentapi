namespace StockportContentApi.Client;

public interface IConverseClient
{
    Task<string> GetLibraries();
    Task<ConverseLibrary> GetLibraryByNameId(string nameId);
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
        return await GetAsync(string.Empty, nameof(GetLibraries));
    }

    public async Task<ConverseLibrary> GetLibraryByNameId(string nameId)
    {
        if (string.IsNullOrWhiteSpace(nameId))
            throw new ArgumentException("A library nameId is required.", nameof(nameId));

        string response = await GetAsync(
            $"library/name-id/{Uri.EscapeDataString(nameId)}",
            nameof(GetLibraryByNameId));

        return JsonConvert.DeserializeObject<ConverseLibrary>(response)
            ?? throw new JsonSerializationException("Converse returned an empty library response.");
    }

    private async Task<string> GetAsync(string requestUri, string methodName)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
            _logger.LogError(
                "{Client}::{Method}: Converse returned {StatusCode} - {ReasonPhrase}",
                nameof(ConverseClient),
                methodName,
                response.StatusCode,
                response.ReasonPhrase);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
