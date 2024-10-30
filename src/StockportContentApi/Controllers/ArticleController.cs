namespace StockportContentApi.Controllers;

public class ArticleController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, ArticleRepository> _createRepository;
    private readonly ArticleRepository _articleRepository;
    private readonly ResponseHandler _handler;
    private readonly IContentfulConfigFactory _configFactory;

    public ArticleController(ResponseHandler handler, ArticleRepository articleRepository, IContentfulConfigFactory configFactory)
    {
        _handler = handler;
        _articleRepository = articleRepository;
        _configFactory = configFactory;
    }

    [HttpGet]
    [Route("{businessId}/articles/{articleSlug}")]
    [Route("v1/{businessId}/articles/{articleSlug}")]
    [Route("v2/{businessId}/articles/{articleSlug}")]
    public async Task<IActionResult> GetArticle(string articleSlug, string businessId)
    {
        var config = _configFactory.CreateConfig(businessId);
        return await _handler.Get(() => _articleRepository.GetArticle(articleSlug));
    }
}