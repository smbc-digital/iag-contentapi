namespace StockportContentApi.Controllers;

public class ArticleController(ResponseHandler handler,
                            Func<string, string, IArticleRepository> createRepository) : Controller
{
    private readonly Func<string, string, IArticleRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/articles/{articleSlug}")]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId, businessId).GetArticle(articleSlug));

    //[ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/articleSiteMap")]
    [Route("v1/{businessId}/articleSiteMap")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() => _createRepository(businessId, businessId).Get());
}