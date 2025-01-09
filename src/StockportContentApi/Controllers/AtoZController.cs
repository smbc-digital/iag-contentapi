namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AtoZController(ResponseHandler handler,
                            Func<string, IAtoZRepository> createRepository) : Controller
{
    private readonly Func<string, IAtoZRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/atoz/{letter}")]
    [Route("v1/{businessId}/atoz/{letter}")]
    public async Task<IActionResult> Index(string letter, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get(letter));
}