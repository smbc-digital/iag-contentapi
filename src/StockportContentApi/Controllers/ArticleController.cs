namespace StockportContentApi.Controllers;

public class ArticleController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, ArticleRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ArticleController(ResponseHandler handler, Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, ArticleRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/articles/{articleSlug}")]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    [Route("v2/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId) =>
        await _handler.Get(() =>
        {
            ArticleRepository repository = _createRepository(_createConfig(businessId));
            return repository.GetArticle(articleSlug);
        });


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/articleSiteMap")]
    [Route("v1/{businessId}/articleSiteMap")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            ArticleRepository repository = _createRepository(_createConfig(businessId));
            return repository.Get();
        });
}