namespace StockportContentApi.Controllers;

public class ArticleController : Controller
{
    private readonly Func<string, IArticleRepository> _createRepository;
    private readonly IResponseHandler _handler;

    public ArticleController(IResponseHandler handler,
        Func<string, IArticleRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId) =>
        await _handler.Get(() =>
        {
            IArticleRepository repository = _createRepository(businessId);
            return repository.GetArticle(articleSlug);
        });


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("v1/{businessId}/articleSiteMap")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            IArticleRepository repository = _createRepository(businessId);
            return repository.Get();
        });
}