namespace StockportContentApi.Controllers;

public class DocumentPageController : Controller
{
    private readonly Func<string, IDocumentPageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public DocumentPageController(ResponseHandler handler,
        Func<string, IDocumentPageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/document-page/{documentPageSlug}")]
    [Route("v1/{businessId}/document-page/{documentPageSlug}")]
    public async Task<IActionResult> GetDocumentPage(string documentPageSlug, string businessId) =>
        await _handler.Get(() =>
        {
            IDocumentPageRepository repository = _createRepository(businessId);

            return repository.GetDocumentPage(documentPageSlug);
        });
}