namespace StockportContentApi.Controllers;

public class ArticleController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<string, CacheKeyConfig> _cacheKeyConfig;
    private readonly Func<ContentfulConfig, CacheKeyConfig, ArticleRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ArticleController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<string, CacheKeyConfig> cacheKeyConfig,
        Func<ContentfulConfig, CacheKeyConfig, ArticleRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _cacheKeyConfig = cacheKeyConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/articles/{articleSlug}")]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId) =>
        await _handler.Get(() =>
        {
            ArticleRepository repository = _createRepository(_createConfig(businessId), _cacheKeyConfig(businessId));

            return repository.GetArticle(articleSlug);
        });


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/articleSiteMap")]
    [Route("v1/{businessId}/articleSiteMap")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            ArticleRepository repository = _createRepository(_createConfig(businessId), _cacheKeyConfig(businessId));

            return repository.Get();
        });
}