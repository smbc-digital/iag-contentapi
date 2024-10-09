namespace StockportContentApi.Http;

[ExcludeFromCodeCoverage]
public class ResponseHandler
{
    private readonly ILogger<ResponseHandler> _logger;

    public ResponseHandler(ILogger<ResponseHandler> logger) =>
        _logger = logger;

    // TODO: Possibly not the most elegant way of doing this.
    public async Task<IActionResult> Get(Func<Task<HttpResponse>> doGet)
    {
        HttpResponse response;

        try
        {
            response = await doGet();
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(0), ex, "An unexpected error occurred while performing the get operation");
            return new StatusCodeResult(500);
        }

        return response.CreateResult();
    }
}