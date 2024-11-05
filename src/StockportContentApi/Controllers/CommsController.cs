namespace StockportContentApi.Controllers;

public class CommsController : Controller
{
    private readonly Func<string, CommsRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public CommsController(ResponseHandler handler,
        Func<string, CommsRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/comms")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            CommsRepository repository = _createRepository(businessId);
            return repository.Get();
        });
}