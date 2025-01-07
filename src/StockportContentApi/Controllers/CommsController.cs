namespace StockportContentApi.Controllers;

public class CommsController(ResponseHandler handler,
                            Func<string, ICommsRepository> commsRepository) : Controller
{
    private readonly Func<string, ICommsRepository> _commsRepository = commsRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/comms")]
    [Route("v1/{businessId}/comms")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _commsRepository(businessId).Get());
}