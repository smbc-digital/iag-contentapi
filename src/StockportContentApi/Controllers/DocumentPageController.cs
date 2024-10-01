namespace StockportContentApi.Controllers;

public class DocumentPageController : Controller
{

    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, DocumentPageRepository> _createRepository;

    public DocumentPageController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, DocumentPageRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/document-page/{documentPageSlug}")]
    [Route("v1/{businessId}/document-page/{documentPageSlug}")]
    [Route("v2/{businessId}/document-page/{documentPageSlug}")]
    public async Task<IActionResult> GetDocumentPage(string documentPageSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            DocumentPageRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> article = repository.GetDocumentPage(documentPageSlug);

            return article;
        });
    }
}