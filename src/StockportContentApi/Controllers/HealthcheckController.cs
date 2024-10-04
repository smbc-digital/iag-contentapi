namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HealthcheckController : Controller
{
    private readonly IHealthcheckService _healthcheckService;

    public HealthcheckController(IHealthcheckService healthcheckService) => _healthcheckService = healthcheckService;

    [HttpGet]
    [Route("/_healthcheck")]
    public async Task<IActionResult> Index() =>
        await Task.Run(async () => Json(await _healthcheckService.Get()));
}