namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AtoZController : Controller
{
    private readonly Func<string, AtoZRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public AtoZController(ResponseHandler handler,
        Func<string, AtoZRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/atoz/{letter}")]
    public async Task<IActionResult> Index(string letter, string businessId) =>
        await _handler.Get(() =>
        {
            AtoZRepository repository = _createRepository(businessId);
            return repository.Get(letter);
        });
}