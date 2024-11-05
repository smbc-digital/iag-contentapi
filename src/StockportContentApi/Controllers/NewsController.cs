namespace StockportContentApi.Controllers;

public class NewsController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, NewsRepository> _newsRepository;

    public NewsController(ResponseHandler handler,
        Func<string, NewsRepository> newsRepository)
    {
        _handler = handler;
        _newsRepository = newsRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/news")]
    public async Task<IActionResult> Index(string businessId,
        [FromQuery] string tag = null,
        [FromQuery] string category = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null) =>
            await _handler.Get(() =>
            {
                NewsRepository repository = _newsRepository(businessId);
                return repository.Get(tag, category, dateFrom, dateTo);
            });

    [HttpGet]
    [Route("v1/{businessId}/news/latest/{limit}")]
    public async Task<IActionResult> LatestNews(string businessId, int limit) =>
        await _handler.Get(() =>
        {
            NewsRepository repository = _newsRepository(businessId);
            return repository.GetNewsByLimit(limit);
        });

    [HttpGet]
    [Route("v1/{businessId}/news/{slug}")]
    public async Task<IActionResult> Detail(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            NewsRepository repository = _newsRepository(businessId);
            return repository.GetNews(slug);
        });
}