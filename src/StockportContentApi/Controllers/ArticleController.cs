namespace StockportContentApi.Controllers;

public class ArticleController : Controller
{
    private readonly Func<string, string, IArticleRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ArticleController(ResponseHandler handler,
        Func<string, string, IArticleRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/articles/{articleSlug}")]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId) =>
        await _handler.Get(() =>
        {
            IArticleRepository repository = _createRepository(businessId, businessId);

            return repository.GetArticle(articleSlug);
        });


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/articleSiteMap")]
    [Route("v1/{businessId}/articleSiteMap")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            IArticleRepository repository = _createRepository(businessId, businessId);

            return repository.Get();
        });
}