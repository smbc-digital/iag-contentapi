namespace StockportContentApi.Controllers;

public class ArticleController : Controller
{
    private readonly Func<string, ArticleRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ArticleController(ResponseHandler handler,
        Func<string, ArticleRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId) =>
        await _handler.Get(() =>
        {
            ArticleRepository repository = _createRepository(businessId);
            return repository.GetArticle(articleSlug);
        });


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("v1/{businessId}/articleSiteMap")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            ArticleRepository repository = _createRepository(businessId);
            return repository.Get();
        });
}