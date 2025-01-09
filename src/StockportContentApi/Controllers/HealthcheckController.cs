namespace StockportContentApi.Controllers;

[ExcludeFromCodeCoverage]
[ApiExplorerSettings(IgnoreApi = true)]
public class HealthcheckController(IHealthcheckService healthcheckService) : Controller
{
    private readonly IHealthcheckService _healthcheckService = healthcheckService;

    [HttpGet]
    [Route("/_healthcheck")]
    public async Task<IActionResult> Index() =>
        await Task.Run(async () => Json(await _healthcheckService.Get()));
}