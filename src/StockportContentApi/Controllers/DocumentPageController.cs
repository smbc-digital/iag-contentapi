namespace StockportContentApi.Controllers;

public class DocumentPageController : Controller
{
    private readonly Func<string, DocumentPageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public DocumentPageController(
        ResponseHandler handler,
        Func<string, DocumentPageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/document-page/{documentPageSlug}")]
    public async Task<IActionResult> GetDocumentPage(string documentPageSlug, string businessId) => 
        await _handler.Get(() =>
        {
            DocumentPageRepository repository = _createRepository(businessId);
            return repository.GetDocumentPage(documentPageSlug);
        });
}