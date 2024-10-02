namespace StockportContentApi.Controllers;

public class RedirectsController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly RedirectsRepository _repository;

    public RedirectsController(ResponseHandler handler,
        RedirectsRepository repository)
    {
        _handler = handler;
        _repository = repository;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("redirects")]
    [Route("v1/redirects")]
    public async Task<IActionResult> GetRedirects() => 
        await _handler.Get(() => _repository.GetRedirects());

    [HttpPatch]
    [Route("v1/redirects")]
    public async Task<IActionResult> UpdateRedirects() => 
        await _handler.Get(() => _repository.GetUpdatedRedirects());
}