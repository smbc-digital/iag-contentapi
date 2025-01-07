namespace StockportContentApi.Controllers;

public class NewsController(ResponseHandler handler,
                            Func<string, INewsRepository> newsRepository) : Controller
{
    private readonly ResponseHandler _handler = handler;
    private readonly Func<string, INewsRepository> _newsRepository = newsRepository;

    [HttpGet]
    [Route("{businessId}/news")]
    [Route("v1/{businessId}/news")]
    public async Task<IActionResult> Index(string businessId,
        [FromQuery] string tag = null,
        [FromQuery] string category = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null) =>
        await _handler.Get(() => _newsRepository(businessId).Get(tag, category, dateFrom, dateTo));

    [HttpGet]
    [Route("{businessId}/news/latest/{limit}")]
    [Route("v1/{businessId}/news/latest/{limit}")]
    public async Task<IActionResult> LatestNews(string businessId, int limit) =>
        await _handler.Get(() => _newsRepository(businessId).GetNewsByLimit(limit));

    [HttpGet]
    [Route("{businessId}/news/{slug}")]
    [Route("v1/{businessId}/news/{slug}")]
    public async Task<IActionResult> Detail(string slug, string businessId) =>
        await _handler.Get(() => _newsRepository(businessId).GetNews(slug));
}