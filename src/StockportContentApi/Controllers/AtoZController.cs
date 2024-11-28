namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AtoZController : Controller
{
    private readonly Func<string, IAtoZRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public AtoZController(ResponseHandler handler,
        Func<string, IAtoZRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/atoz/{letter}")]
    [Route("v1/{businessId}/atoz/{letter}")]
    public async Task<IActionResult> Index(string letter, string businessId) =>
        await _handler.Get(() =>
        {
            IAtoZRepository repository = _createRepository(businessId);

            return repository.Get(letter);
        });
}